namespace DeliveryPlatform.Data.Entities
{
    public class OrderLineOption : BaseEntity
    {
        public int OrderLineId { get; set; }
        public OrderLine OrderLine { get; set; } = null!;

        public int ProductOptionId { get; set; }
        public ProductOption ProductOption { get; set; } = null!;
    }
}