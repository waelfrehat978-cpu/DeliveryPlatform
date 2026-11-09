namespace DeliveryPlatform.Service.DTOs.Products
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int StoreId { get; set; }

        public int CategoryId { get; set; }
    }
}