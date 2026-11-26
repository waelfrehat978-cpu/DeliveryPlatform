using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Stores
{
    public class UpdateStoreNameDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}