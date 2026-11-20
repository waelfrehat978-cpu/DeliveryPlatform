using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Customers
{
    public class UpdateCustomerDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{10,15}$", ErrorMessage = "PhoneNumber must be 10 to 15 digits, and nothing else.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;
    }
}