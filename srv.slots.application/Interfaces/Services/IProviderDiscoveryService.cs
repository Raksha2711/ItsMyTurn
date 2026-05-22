using srv.slots.application.DTOs.Provider;

namespace srv.slots.application.Interfaces.Services;

public interface IProviderDiscoveryService
{
    Task<List<ServiceDomainDto>> GetCategoryTreeAsync();
    Task<PagedResult<ProviderSummaryDto>> SearchAsync(SearchProvidersDto filter);
    Task<ProviderDetailDto> GetDetailAsync(uint providerId);
}
