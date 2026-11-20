
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DeliveryPlatform.Service.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtTokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("role", user.Role.ToString())
            };

            if (user.Role == UserRole.Partner && user.StoreId.HasValue)
            {
                claims.Add(new Claim("storeId", user.StoreId.Value.ToString()));
            }

            if (user.Role == UserRole.Customer && user.CustomerId.HasValue)
            {
                claims.Add(new Claim("customerId", user.CustomerId.Value.ToString()));
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    _jwtOptions.AccessTokenMinutes),
                SigningCredentials = credentials
            };

            var handler = new JsonWebTokenHandler();

            return handler.CreateToken(descriptor);
        }
    }
}
