namespace DeliveryPlatform.Data.Entities
{
    public class CartLineOption : BaseEntity
    {
        public int CartLineId { get; set; }
        public CartLine CartLine { get; set; } = null!;

        public int ProductOptionId { get; set; }
        public ProductOption ProductOption { get; set; } = null!;
    }
}