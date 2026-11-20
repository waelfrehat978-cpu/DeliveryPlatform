using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.DTOs.Auth;

namespace DeliveryPlatform.Service.Interfaces;

public interface IRefreshTokenService
{
  

    Task<TokenResponseDto> IssueTokenPairAsync(User user);

    Task<TokenResponseDto> RefreshAsync(string refreshToken);

    Task RevokeAsync(int userId, string refreshToken);
}
/*
 
IssueTokenPairAsync: إصدار Access Token وRefresh Token معًا.



RefreshAsync: تجديد التوكنات باستخدام Refresh Token.



RevokeAsync: إلغاء Refresh Token الخاص بالمستخدم عند تسجيل الخروج. 

 */