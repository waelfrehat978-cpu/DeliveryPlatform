using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.DTOs.Auth;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Security;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Service.Options;
using Microsoft.Extensions.Options;

namespace DeliveryPlatform.Service.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IClockService _clock;
    private readonly int _refreshTokenDays;
    private readonly DeliveryPlatformDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    public RefreshTokenService(
      DeliveryPlatformDbContext context,
      IJwtTokenService jwtTokenService,
      IOptions<JwtOptions> jwtOptions,
      IClockService clock)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _refreshTokenDays = jwtOptions.Value.RefreshTokenDays;
        _clock = clock;
    }

    public async Task<TokenResponseDto> IssueTokenPairAsync(User user)
    {
        var refreshToken = SecureToken.Create();

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = SecureToken.Hash(refreshToken),
            UserId = user.Id,
            ExpiresAtUtc = _clock.UtcNow.AddDays(_refreshTokenDays)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = _jwtTokenService.GenerateAccessToken(user),
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponseDto> RefreshAsync(string refreshToken)
    {
        var tokenHash = SecureToken.Hash(refreshToken);

        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken == null)
            throw new UnauthorizedException("Invalid refresh token.");

        var now = _clock.UtcNow;

        if (storedToken.RevokedAtUtc.HasValue)
        {
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == storedToken.UserId)
                .ToListAsync();

            foreach (var token in userTokens)
                token.RevokedAtUtc ??= now;

            await _context.SaveChangesAsync();

            throw new UnauthorizedException("Refresh token reuse detected.");
        }

        if (storedToken.ExpiresAtUtc <= now)
            throw new UnauthorizedException("Refresh token expired.");

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var newRefreshToken = SecureToken.Create();

        var newTokenEntity = new RefreshToken
        {
            TokenHash = SecureToken.Hash(newRefreshToken),
            UserId = storedToken.UserId,
            ExpiresAtUtc = now.AddDays(_refreshTokenDays)
        };

        _context.RefreshTokens.Add(newTokenEntity);

        await _context.SaveChangesAsync();

        storedToken.RevokedAtUtc = now;
        storedToken.ReplacedByTokenId = newTokenEntity.Id;

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return new TokenResponseDto
        {
            AccessToken = _jwtTokenService.GenerateAccessToken(storedToken.User),
            RefreshToken = newRefreshToken
        };
    }

    public async Task RevokeAsync(int userId, string refreshToken)
    {
        var tokenHash = SecureToken.Hash(refreshToken);
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == tokenHash &&
                rt.UserId == userId);

        if (storedToken == null || storedToken.RevokedAtUtc.HasValue)
            return;

        storedToken.RevokedAtUtc = _clock.UtcNow;

        await _context.SaveChangesAsync();
    }
}