using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Data.Auditing;

public abstract class AuditTrail
{
    public long Id { get; set; }

    public string EntityName { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public AuditAction Action { get; set; }

    public string ChangesJson { get; set; } = null!;

    public DateTime TimestampUtc { get; set; }

    public int? UserId { get; set; }
}

/*

. EntityName

اسم نوع الكيان الذي تغير، مثل Product أو Store.

. EntityId

رقم السجل الذي تغير. استخدمنا string حتى نستطيع تخزين أنواع مختلفة من المفاتيح.

. Action

يحدد نوع التغيير، ويستخدم الـ enum الذي أنشأناه:
- Update = 1: تعديل.
- SoftDelete = 2: حذف منطقي.

. ChangesJson

يخزن تفاصيل التغيير بصيغة JSON.

مثلاً، بصورة توضيحية:
{
  "Price": {
    "Old": 100,
    "New": 120
  }
}


6. TimestampUtc

وقت حدوث التغيير بتوقيت UTC.

7. UserId

رقم المستخدم الذي نفّذ التغيير. جعلناه int? لأن المستخدم قد لا يكون مسجل الدخول.
 
 */