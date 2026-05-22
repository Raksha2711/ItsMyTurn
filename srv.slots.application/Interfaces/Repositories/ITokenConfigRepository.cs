using srv.slots.application.DTOs.TokenConfig;

namespace srv.slots.application.Interfaces.Repositories;

public interface ITokenConfigRepository
{
    Task<TokenConfigDto?> GetByProviderAsync(uint providerId);
    Task UpsertAsync(uint providerId, UpsertTokenConfigDto dto);
}
