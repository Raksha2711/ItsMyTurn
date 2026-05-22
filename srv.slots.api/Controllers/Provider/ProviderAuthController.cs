using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Auth;
using srv.slots.application.DTOs.ProviderAuth;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/auth")]
public class ProviderAuthController : ControllerBase
{
    private readonly IProviderAuthService _auth;

    public ProviderAuthController(IProviderAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] ProviderSignupDto dto)
    {
        var result = await _auth.SignupAsync(dto, GetIp(), GetDevice());
        return Ok(ApiResponse<ProviderAuthResponseDto>.Ok(result, result.Message));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ProviderLoginDto dto)
    {
        var result = await _auth.LoginAsync(dto, GetIp(), GetDevice());
        return Ok(ApiResponse<ProviderAuthResponseDto>.Ok(result, result.Message));
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] ProviderSendOtpDto dto)
    {
        var code = await _auth.SendOtpAsync(dto);
        return Ok(ApiResponse<object>.Ok(new { OtpForTesting = code }, "OTP sent."));
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] ProviderVerifyOtpDto dto)
    {
        var result = await _auth.VerifyOtpAsync(dto, GetIp(), GetDevice());
        return Ok(ApiResponse<ProviderAuthResponseDto>.Ok(result, result.Message));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _auth.RefreshTokenAsync(dto.RefreshToken, GetIp(), GetDevice());
        return Ok(ApiResponse<ProviderAuthResponseDto>.Ok(result, "Token refreshed."));
    }

    [HttpPost("logout")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        await _auth.LogoutAsync(dto.RefreshToken);
        return Ok(ApiResponse.OkMsg("Logged out."));
    }

    private string? GetIp() => HttpContext.Connection.RemoteIpAddress?.ToString();
    private string? GetDevice() => Request.Headers.UserAgent.ToString();
}
