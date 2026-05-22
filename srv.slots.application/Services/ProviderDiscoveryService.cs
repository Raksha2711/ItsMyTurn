using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Provider;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class ProviderDiscoveryService : IProviderDiscoveryService
{
    private readonly IProviderRepository _providerRepo;
    private readonly ICategoryRepository _categoryRepo;

    public ProviderDiscoveryService(IProviderRepository providerRepo, ICategoryRepository categoryRepo)
    {
        _providerRepo = providerRepo;
        _categoryRepo = categoryRepo;
    }

    public Task<List<ServiceDomainDto>> GetCategoryTreeAsync()
        => _categoryRepo.GetAllWithCategoriesAsync();

    public Task<PagedResult<ProviderSummaryDto>> SearchAsync(SearchProvidersDto filter)
        => _providerRepo.SearchAsync(filter);

    public async Task<ProviderDetailDto> GetDetailAsync(uint providerId)
    {
        return await _providerRepo.GetDetailAsync(providerId)
            ?? throw new NotFoundException("Provider not found.");
    }
}
