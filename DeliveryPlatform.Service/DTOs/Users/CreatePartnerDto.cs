using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Users;

public class CreatePartnerDto
{
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int StoreId { get; set; }
}