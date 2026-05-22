using srv.slots.application.DTOs.ProviderProfile;

namespace srv.slots.application.Interfaces.Services;

public interface IProviderSelfProfileService
{
    Task<ProviderProfileDto> GetAsync(uint providerId);
    Task UpdateAsync(uint providerId, UpdateProviderProfileDto dto);
}
