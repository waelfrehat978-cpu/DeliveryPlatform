using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Service.DTOs.AuditHistory
{
    public class AuditHistoryDto
    {
        public DateTime TimestampUtc { get; set; }

        public AuditAction Action { get; set; }

        public int? UserId { get; set; }

        public string UserDisplayName { get; set; } = string.Empty;

        public List<AuditChangeDto> Changes { get; set; } = new();
    }
}/*يمثل سجل تدقيق واحد كاملاً:
  * - TimestampUtc: وقت التغيير.
- Action: هل العملية Update أو SoftDelete؟
- UserId: رقم المستخدم الذي أجرى التغيير.
- UserDisplayName: اسم المستخدم أو إيميله، أو System.
- Changes: قائمة التغييرات، وكل عنصر فيها يحتوي الخاصية والقيمة القديمة والجديدة.
ليش استخدمنا object? للقيم؟

لأن القيم مش كلها من نفس النوع؛ السعر decimal، وIsDeleted من النوع bool، والاسم string.
 
 */