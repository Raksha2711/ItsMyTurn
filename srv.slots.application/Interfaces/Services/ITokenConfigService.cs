using srv.slots.application.DTOs.TokenConfig;

namespace srv.slots.application.Interfaces.Services;

public interface ITokenConfigService
{
    Task<TokenConfigDto> GetAsync(uint providerId);
    Task UpsertAsync(uint providerId, UpsertTokenConfigDto dto);
}
