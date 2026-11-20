
namespace DeliveryPlatform.Service.DTOs.Addresses
{
    public class AddressResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public bool IsDefault { get; set; }
    }
}
