using srv.slots.application.DTOs.Provider;

namespace srv.slots.application.Interfaces.Repositories;

public interface IProviderRepository
{
    Task<PagedResult<ProviderSummaryDto>> SearchAsync(SearchProvidersDto filter);
    Task<ProviderDetailDto?> GetDetailAsync(uint providerId);
}
