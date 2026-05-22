using srv.slots.application.DTOs.ProviderService;

namespace srv.slots.application.Interfaces.Services;

public interface IProviderServiceManagementService
{
    Task<List<ProviderServiceDto>> GetAllAsync(uint providerId);
    Task<ProviderServiceDto> GetByIdAsync(uint id, uint providerId);
    Task<ProviderServiceDto> CreateAsync(uint providerId, UpsertProviderServiceDto dto);
    Task<ProviderServiceDto> UpdateAsync(uint id, uint providerId, UpsertProviderServiceDto dto);
    Task DeleteAsync(uint id, uint providerId);
    Task ToggleActiveAsync(uint id, uint providerId);
}
