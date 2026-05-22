using srv.slots.domain.Entities;
using srv.slots.domain.Enums;

namespace srv.slots.application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<ulong> CreateAsync(RefreshToken token);
    Task<RefreshToken?> GetActiveByHashAsync(string tokenHash);
    Task RevokeAsync(ulong id);
    Task RevokeAllForUserAsync(UserType userType, uint userId);
}
