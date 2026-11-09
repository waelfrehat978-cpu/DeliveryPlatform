namespace DeliveryPlatform.Data.Entities
{
    public class ProductOptionGroup : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}