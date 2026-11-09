using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Addresses
{
    public class CreateAddressRequestDto
    {
       
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }
        [Required, MaxLength(50)]
        public string Label { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string Street { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Range(-90, 90)]
        public decimal Latitude { get; set; }
        [Range(-180, 180)]
        public decimal Longitude { get; set; }

        public bool IsDefault { get; set; }
    }
}