namespace DeliveryPlatform.Service.DTOs.AuditHistory
{
    public class AuditChangeDto
    {
        public string Property { get; set; } = string.Empty;

        public object? OldValue { get; set; }

        public object? NewValue { get; set; }
    }
}//يمثل تغيير خاصية واحدة، مثل تغيير السعر: