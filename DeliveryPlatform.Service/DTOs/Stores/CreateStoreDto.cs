
using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Stores;

public class CreateStoreDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(-90, 90)]
    public decimal Latitude { get; set; }

    [Range(-180, 180)]
    public decimal Longitude { get; set; }
}
