using Microsoft.AspNetCore.Mvc;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Auth;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Customer;

[ApiController]
[Route("api/customer/auth")]
public class AuthController : ControllerBase
{
    private readonly ICustomerAuthService _auth;

    public AuthController(ICustomerAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequestDto dto)
    {
        var result = await _auth.SignupAsync(dto, GetIp(), GetDevice());
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Signup successful."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _auth.LoginAsync(dto, GetIp(), GetDevice());
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDto dto)
    {
        var code = await _auth.SendOtpAsync(dto);
        // In production we'd return only a success message. For dev/testing the code is returned.
        return Ok(ApiResponse<object>.Ok(new { OtpForTesting = code }, "OTP sent."));
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
    {
        var result = await _auth.VerifyOtpAsync(dto, GetIp(), GetDevice());
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "OTP verified."));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _auth.RefreshTokenAsync(dto.RefreshToken, GetIp(), GetDevice());
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Token refreshed."));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        await _auth.LogoutAsync(dto.RefreshToken);
        return Ok(ApiResponse.OkMsg("Logged out."));
    }

    private string? GetIp() => HttpContext.Connection.RemoteIpAddress?.ToString();
    private string? GetDevice() => Request.Headers.UserAgent.ToString();
}
