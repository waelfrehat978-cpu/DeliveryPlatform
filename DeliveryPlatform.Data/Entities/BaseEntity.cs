

namespace DeliveryPlatform.Data.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAtUtc { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}/*
  Property	معناها
CreatedByUserId	المستخدم الذي أنشأ السجل
UpdatedByUserId	المستخدم الذي عدّل السجل
DeletedByUserId	المستخدم الذي حذف السجل

لماذا int?؟

لأن User.Id عندك من نوع int، واستخدمنا ? لأن العملية قد تحدث بدون مستخدم مسجل الدخول
  
  */
