namespace DeliveryPlatform.Data.Entities
{
    public class ProductOption : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public decimal AdditionalPrice { get; set; }

        public int ProductOptionGroupId { get; set; }

        public ProductOptionGroup ProductOptionGroup
        {
            get; set;
        } = null!;
    }
}