using srv.slots.application.DTOs.ProviderProfile;

namespace srv.slots.application.Interfaces.Repositories;

public interface IProviderSelfProfileRepository
{
    Task<ProviderProfileDto?> GetProfileAsync(uint providerId);
    Task<bool> UpdateProfileAsync(uint providerId, UpdateProviderProfileDto dto);
}
