using srv.slots.domain.Enums;

namespace srv.slots.application.Interfaces.Services;

public interface IJwtTokenService
{
    (string token, DateTime expiresAt) GenerateAccessToken(uint userId, string mobile, UserType userType);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
