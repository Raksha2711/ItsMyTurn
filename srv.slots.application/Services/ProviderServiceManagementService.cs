using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.ProviderService;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class ProviderServiceManagementService : IProviderServiceManagementService
{
    private readonly IProviderServiceRepository _repo;

    public ProviderServiceManagementService(IProviderServiceRepository repo) => _repo = repo;

    public Task<List<ProviderServiceDto>> GetAllAsync(uint providerId)
        => _repo.GetAllByProviderAsync(providerId);

    public async Task<ProviderServiceDto> GetByIdAsync(uint id, uint providerId)
        => await _repo.GetByIdAsync(id, providerId) ?? throw new NotFoundException("Service not found.");

    public async Task<ProviderServiceDto> CreateAsync(uint providerId, UpsertProviderServiceDto dto)
    {
        var id = await _repo.CreateAsync(providerId, dto);
        return await GetByIdAsync(id, providerId);
    }

    public async Task<ProviderServiceDto> UpdateAsync(uint id, uint providerId, UpsertProviderServiceDto dto)
    {
        var exists = await _repo.ExistsForProviderAsync(id, providerId);
        if (!exists) throw new NotFoundException("Service not found.");

        var ok = await _repo.UpdateAsync(id, providerId, dto);
        if (!ok) throw new AppException("Could not update service.");

        return await GetByIdAsync(id, providerId);
    }

    public async Task DeleteAsync(uint id, uint providerId)
    {
        var ok = await _repo.SoftDeleteAsync(id, providerId);
        if (!ok) throw new NotFoundException("Service not found.");
    }

    public async Task ToggleActiveAsync(uint id, uint providerId)
    {
        var ok = await _repo.ToggleActiveAsync(id, providerId);
        if (!ok) throw new NotFoundException("Service not found.");
    }
}
