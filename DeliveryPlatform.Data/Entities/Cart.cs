namespace DeliveryPlatform.Data.Entities
{
    public class Cart : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int StoreId { get; set; }
        public Store Store { get; set; } = null!;
    }
}