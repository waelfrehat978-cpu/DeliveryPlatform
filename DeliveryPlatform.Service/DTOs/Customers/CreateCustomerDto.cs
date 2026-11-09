using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Customers
{
    public class CreateCustomerDto
    {
        [Required]//ممنوع يكون فارغ
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;


        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;


        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string? Email { get; set; }
    }
}