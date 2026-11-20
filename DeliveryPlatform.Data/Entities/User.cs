using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Data.Entities;

public class User : BaseEntity
{
    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int? StoreId { get; set; }
    public Store? Store { get; set; }
}