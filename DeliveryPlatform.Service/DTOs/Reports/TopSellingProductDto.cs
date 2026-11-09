namespace DeliveryPlatform.Service.DTOs.Reports
{
    public class TopSellingProductDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int TotalQuantitySold { get; set; }
    }
}