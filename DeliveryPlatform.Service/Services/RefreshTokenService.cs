using System.Security.Cryptography;
using System.Text;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.DTOs.Auth;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly DeliveryPlatformDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenService(
        DeliveryPlatformDbContext context,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashRefreshToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public async Task<TokenResponseDto> IssueTokenPairAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = HashRefreshToken(refreshToken),
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
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
        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken == null)
            throw new UnauthorizedException("Invalid refresh token.");

        var now = DateTime.UtcNow;

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

        var newRefreshToken = GenerateRefreshToken();

        var newTokenEntity = new RefreshToken
        {
            TokenHash = HashRefreshToken(newRefreshToken),
            UserId = storedToken.UserId,
            ExpiresAtUtc = now.AddDays(7)
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
        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == tokenHash &&
                rt.UserId == userId);

        if (storedToken == null || storedToken.RevokedAtUtc.HasValue)
            return;

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}