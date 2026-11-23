
using DeliveryPlatform.Data.Auditing;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;

namespace DeliveryPlatform.Data.Interceptors
{
    public class AppSaveChangesInterceptor : SaveChangesInterceptor
    {
        /*
         static: حتى تكون الذاكرة مشتركة بين جميع نسخ Interceptor، ولا تُعاد مع كل Request.
ConcurrentDictionary: يسمح لعدة Requests باستخدام الـ Cache بأمان.
Type: المفتاح هو نوع الـ Entity، مثل Product أو Store.
Lazy: يجعل قراءة الـ Attribute تحدث مرة واحدة فقط، ويسمح لنا بتخزين نتيجة null أيضًا عندما لا يحتوي الكلاس على Attribute.
         */
        private static readonly ConcurrentDictionary<Type, Lazy<AuditedInAttribute?>> _attributeCache = new();
        private readonly ICurrentUser _currentUser;
        private readonly IAuditScope _auditScope;

        public AppSaveChangesInterceptor(
            ICurrentUser currentUser,
            IAuditScope auditScope)
        {
            _currentUser = currentUser;
            _auditScope = auditScope;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            // نتأكد أن DbContext موجود.
            if (context is null)
                return;

            // وقت موحد لجميع التغييرات في عملية الحفظ الحالية.
            var now = DateTime.UtcNow;

            // قائمة لتجميع سجلات التدقيق قبل إضافتها إلى DbContext.
            var auditRows = new List<AuditTrail>();

            // نحصل على الكيانات التي ترث من BaseEntity.
            // ToList تنشئ نسخة من القائمة حتى لا تتأثر أثناء العمل.
            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>().ToList())
            {
                // نحفظ الحالة الأصلية قبل أن نغيّرها، خصوصًا عند الحذف.
                var originalState = entry.State;

                // تحديث بيانات الوقت والمستخدم حسب حالة الكيان.
                switch (entry.State)
                {
                    case EntityState.Added:
                        // تسجيل وقت إنشاء السجل والمستخدم الذي أنشأه.
                        entry.Entity.CreatedAtUtc = now;
                        entry.Entity.CreatedByUserId = _currentUser.UserId;
                        break;

                    case EntityState.Modified:
                        // تسجيل وقت التعديل والمستخدم الذي قام به.
                        entry.Entity.UpdatedByUserId = _currentUser.UserId;
                        entry.Entity.UpdatedAtUtc = now;
                        break;

                    case EntityState.Deleted:
                        // تسجيل المستخدم الذي طلب الحذف.
                        entry.Entity.DeletedByUserId = _currentUser.UserId;

                        // تحويل الحذف الفعلي إلى Soft Delete.
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAtUtc = now;
                        break;
                }

                // إذا كان التدقيق غير مفعّل للعملية، نتجاوز الكيان.
                if (!_auditScope.IsEnabled)
                    continue;

                // لا نسجل عمليات الإنشاء في هذه المهمة.
                if (originalState == EntityState.Added)
                    continue;

                /*أول مرة: يبحث عن الـ Attribute ويحفظ النتيجة.
المرات التالية: يسترجع النتيجة من الـ Cache.
إذا لم يجد Attribute، يحفظ نتيجة البحث أيضًا.*/
                var attribute = GetAuditedAttribute(entry.Metadata.ClrType);
                // إذا لم يكن الكيان يحتوي على Attribute، نتجاوزه.
                if (attribute is null)
                    continue;

                // بناء قاموس يحتوي الخصائص التي تغيرت فعلًا.
                var changes = entry.Properties
                    .Where(p =>
                        p.IsModified &&
                        !Equals(p.OriginalValue, p.CurrentValue) &&

                        // استثناء UpdatedAtUtc لأنه يتغير تلقائيًا.
                        p.Metadata.Name != nameof(BaseEntity.UpdatedAtUtc) &&

                        // استثناء UpdatedByUserId لأنه يتغير تلقائيًا.
                        p.Metadata.Name != nameof(BaseEntity.UpdatedByUserId))
                    .ToDictionary(
                        p => p.Metadata.Name,
                        p => new
                        {
                            // القيمة قبل التعديل.
                            Old = p.OriginalValue,

                            // القيمة بعد التعديل.
                            New = p.CurrentValue
                        });

                // إذا لم توجد تغييرات فعلية، لا ننشئ سجل تدقيق.
                if (changes.Count == 0)
                    continue;

                // إنشاء كائن من نوع جدول التدقيق المحدد داخل Attribute.
                var auditRow = (AuditTrail)Activator.CreateInstance(attribute.AuditTableType)!;

                // اسم الكيان الذي تم تعديله، مثل Product.
                auditRow.EntityName = entry.Metadata.ClrType.Name;
                

                // رقم السجل الذي تم تعديله وتحويله إلى string.
                auditRow.EntityId = entry.Property("Id").CurrentValue!.ToString()!;

                auditRow.Action = originalState == EntityState.Deleted
      ? AuditAction.SoftDelete
      : AuditAction.Update;

                // تحويل التغييرات إلى JSON لتخزين القيم القديمة والجديدة.
                auditRow.ChangesJson =
                    System.Text.Json.JsonSerializer.Serialize(changes);

                // وقت حدوث التغيير.
                auditRow.TimestampUtc = now;

                // المستخدم الذي قام بالتغيير، وقد تكون القيمة null إذا كان النظام.
                auditRow.UserId = _currentUser.UserId;

                // إضافة سجل التدقيق إلى القائمة، وليس إلى DbContext مباشرة.
                auditRows.Add(auditRow);
            }

            // بعد انتهاء الحلقة، نضيف جميع سجلات التدقيق دفعة واحدة.
            // هذا يمنع تعديل ChangeTracker أثناء المرور على الكيانات.
            if (auditRows.Count > 0)
            {
                context.AddRange(auditRows);
            }
        }
        private static AuditedInAttribute? GetAuditedAttribute(Type entityType)
        {
            return _attributeCache.GetOrAdd(
                entityType,
                type => new Lazy<AuditedInAttribute?>(() =>
                    type.GetCustomAttribute<AuditedInAttribute>(false)

                )
            ).Value;
        }
        /*
         entityType: نوع الـ Entity الذي نبحث عنه.
GetOrAdd: يبحث في الـ Cache؛ إذا وجد النوع يعيد النتيجة، وإذا لم يجده يضيفها.
GetCustomAttribute: يبحث عن [AuditedIn] باستخدام Reflection.
Lazy: يضمن تنفيذ عملية البحث مرة واحدة عند الحاجة.
.Value: يعيد نتيجة البحث، وقد تكون null.
         */
    }
}
