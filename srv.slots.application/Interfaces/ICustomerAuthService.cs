using srv.slots.application.DTOs.Auth;

namespace srv.slots.application.Interfaces.Services;

public interface ICustomerAuthService
{
    Task<AuthResponseDto> SignupAsync(SignupRequestDto dto, string? ipAddress, string? deviceInfo);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, string? ipAddress, string? deviceInfo);
    Task<string> SendOtpAsync(SendOtpRequestDto dto);
    Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto dto, string? ipAddress, string? deviceInfo);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress, string? deviceInfo);
    Task LogoutAsync(string refreshToken);
}
