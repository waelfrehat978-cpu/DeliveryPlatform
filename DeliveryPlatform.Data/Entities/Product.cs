using DeliveryPlatform.Data.Auditing;
namespace DeliveryPlatform.Data.Entities
{
    [AuditedIn(typeof(ProductAuditTrail))]
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int StoreId { get; set; }

        public Store Store { get; set; } = null!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}