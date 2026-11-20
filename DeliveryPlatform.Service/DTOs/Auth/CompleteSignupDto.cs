
namespace DeliveryPlatform.Service.DTOs.Auth;

public class CompleteSignupDto
{
    public string PhoneNumber { get; set; } = string.Empty;//قم الهاتف الذي تم التحقق منه.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
