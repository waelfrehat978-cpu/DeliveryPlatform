using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Data.Interfaces;
namespace DeliveryPlatform.Api.Services;

public class CurrentUser : ICurrentUser
{
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor
      _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated == true;

    public int? UserId =>
        IsAuthenticated &&
        int.TryParse(Principal?.FindFirst("sub")?.Value, out var id)
            ? id
            : null;

    public UserRole? Role =>
        IsAuthenticated &&
        Enum.TryParse<UserRole>(
            Principal?.FindFirst("role")?.Value,
            out var role)
                ? role
                : null;

    public int? StoreId =>
        Role == UserRole.Partner &&
        int.TryParse(Principal?.FindFirst("storeId")?.Value, out var id)
            ? id
            : null;

    public int? CustomerId =>
        Role == UserRole.Customer &&
        int.TryParse(Principal?.FindFirst("customerId")?.Value, out var id)
            ? id
            : null;
}
