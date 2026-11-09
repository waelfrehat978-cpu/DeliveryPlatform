using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Customers
{
    public class CustomerDto
    {
        public int Id { get; set; }

      
        public string FirstName { get; set; } = string.Empty;

    
        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
    }
}