using srv.slots.application.DTOs.ProviderAuth;

namespace srv.slots.application.Interfaces.Services;

public interface IProviderAuthService
{
    Task<ProviderAuthResponseDto> SignupAsync(ProviderSignupDto dto, string? ip, string? device);
    Task<ProviderAuthResponseDto> LoginAsync(ProviderLoginDto dto, string? ip, string? device);
    Task<string> SendOtpAsync(ProviderSendOtpDto dto);
    Task<ProviderAuthResponseDto> VerifyOtpAsync(ProviderVerifyOtpDto dto, string? ip, string? device);
    Task<ProviderAuthResponseDto> RefreshTokenAsync(string refreshToken, string? ip, string? device);
    Task LogoutAsync(string refreshToken);

    Task<ProviderStatusDto> GetStatusAsync(uint providerId);
    Task ResubmitAsync(uint providerId, ProviderResubmitDto dto);
}
