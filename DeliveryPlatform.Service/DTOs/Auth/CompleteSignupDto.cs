
namespace DeliveryPlatform.Service.DTOs.Auth;

public class CompleteSignupDto
{
    public string SignupToken { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
