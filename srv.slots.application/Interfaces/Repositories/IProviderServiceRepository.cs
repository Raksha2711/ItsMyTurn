using srv.slots.application.DTOs.ProviderService;

namespace srv.slots.application.Interfaces.Repositories;

public interface IProviderServiceRepository
{
    Task<List<ProviderServiceDto>> GetAllByProviderAsync(uint providerId);
    Task<ProviderServiceDto?> GetByIdAsync(uint id, uint providerId);
    Task<bool> ExistsForProviderAsync(uint id, uint providerId);
    Task<uint> CreateAsync(uint providerId, UpsertProviderServiceDto dto);
    Task<bool> UpdateAsync(uint id, uint providerId, UpsertProviderServiceDto dto);
    Task<bool> SoftDeleteAsync(uint id, uint providerId);
    Task<bool> ToggleActiveAsync(uint id, uint providerId);
}
