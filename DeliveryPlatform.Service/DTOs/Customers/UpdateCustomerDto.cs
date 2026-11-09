using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Customers
{
    public class UpdateCustomerDto
    {
        [Required,MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(20),Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required, MaxLength(254),EmailAddress]
        public string? Email { get; set; }
    }
}