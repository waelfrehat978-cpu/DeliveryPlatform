namespace DeliveryPlatform.Data.Auditing;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AuditedInAttribute : Attribute
{
    public Type AuditTableType { get; }

    public AuditedInAttribute(Type auditTableType)
    {
        if (!typeof(AuditTrail).IsAssignableFrom(auditTableType))
        {
            throw new ArgumentException(
                $"{auditTableType.Name} must inherit AuditTrail.");
        }

        AuditTableType = auditTableType;
    }
}/*
  Attribute: يجعل الكلاس قابلًا للاستخدام كـ Attribute على الكلاسات الأخرى.



AttributeTargets.Class: يسمح باستخدامه على الكلاسات فقط.



Inherited = false: لا ينتقل تلقائيًا إلى الكلاسات المشتقة.



sealed: يمنع إنشاء كلاسات ترث من هذا الـ Attribute.



Type AuditTableType: يخزن نوع جدول التدقيق المطلوب.



IsAssignableFrom: يتأكد أن النوع الذي مررناه يرث من AuditTrail.
  */