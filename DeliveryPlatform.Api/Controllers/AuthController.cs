using DeliveryPlatform.Service.DTOs.Auth;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeliveryPlatform.Service.DTOs.Users;
using DeliveryPlatform.Data.Interfaces;


namespace DeliveryPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IOtpService _otpService;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    public AuthController(
     IOtpService otpService,
     IUserService userService,
     IRefreshTokenService refreshTokenService, ICurrentUser currentUser)
    {
        _otpService = otpService;
   
        _userService = userService;
        _refreshTokenService = refreshTokenService;
        _currentUser = currentUser;
    }

    [AllowAnonymous]
    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp(
        [FromBody] RequestOtpDto request)
    {
        await _otpService.RequestCodeAsync(request.PhoneNumber);

        return Ok(new
        {
            Message = "If this phone number is valid, a code was sent."
        });
    }

    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpDto request)
    {
        var user = await _otpService.VerifyCodeAsync(
            request.PhoneNumber,
            request.Code);

        if (user == null)
        {
            return Ok(new
            {
                Message = "Phone verified. Please complete your signup."
            });
        }

        var tokens = await _refreshTokenService.IssueTokenPairAsync(user);
        return Ok(new
        {
            Message = "Phone verified successfully.",
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.TokenType
        });
    }

    [AllowAnonymous]
    [HttpPost("complete-signup")]
    public async Task<IActionResult> CompleteSignup(
        [FromBody] CompleteSignupDto request)
    {
        var user = await _otpService.CompleteSignupAsync(
      request.PhoneNumber,
      request.FirstName,
      request.LastName,
      request.Email);

        var tokens = await _refreshTokenService.IssueTokenPairAsync(user);
        return Ok(new
        {
            Message = "Customer account created successfully.",
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.TokenType
        });
    }
    [AllowAnonymous]
    [HttpPost("staff-login")]
    public async Task<IActionResult> StaffLogin(
    [FromBody] StaffLoginDto request)
    {
        var user = await _userService.LoginStaffAsync(
            request.Email,
            request.Password);

        var tokens = await _refreshTokenService.IssueTokenPairAsync(user);

        return Ok(new
        {
            Message = "Login successful.",
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.TokenType
        });
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    [FromBody] RefreshTokenRequestDto request)
    {
        if (!_currentUser.UserId.HasValue)
            return Unauthorized();

        await _refreshTokenService.RevokeAsync(
            _currentUser.UserId.Value,
            request.RefreshToken);

        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("create-partner")]
    public async Task<IActionResult> CreatePartner(
    [FromBody] CreatePartnerDto request)
    {
        var partner = await _userService.CreatePartnerAsync(request);

        return Created(
            $"/api/v1/users/{partner.Id}",
            new
            {
                Message = "Partner account created successfully.",
                partner.Id,
                partner.Email,
                partner.StoreId,
                Role = partner.Role.ToString()
            });
    }
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
    [FromBody] RefreshTokenRequestDto request)
    {
        var tokens = await _refreshTokenService.RefreshAsync(
            request.RefreshToken);

        return Ok(tokens);
    }
}
