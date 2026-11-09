namespace DeliveryPlatform.Service.DTOs.Stores
{
    public class StoreDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int ProductCount { get; set; }
    }
}