using srv.slots.application.DTOs.Public;
using srv.slots.application.DTOs.Provider;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class LookupService : ILookupService
{
    private readonly ILookupRepository _repo;
    public LookupService(ILookupRepository repo) => _repo = repo;

    public Task<List<CountryDto>> GetCountriesAsync() => _repo.GetCountriesAsync();
    public Task<List<StateDto>> GetStatesAsync(uint countryId) => _repo.GetStatesAsync(countryId);
    public Task<List<CityDto>> GetCitiesAsync(uint stateId) => _repo.GetCitiesAsync(stateId);
    public Task<List<BoundaryDto>> GetBoundariesAsync(uint cityId) => _repo.GetBoundariesByCityAsync(cityId);
    public Task<List<ServiceDomainDto>> GetDomainsAsync() => _repo.GetActiveDomainsAsync();
    public Task<List<ServiceCategoryDto>> GetCategoriesAsync(uint domainId) => _repo.GetCategoriesByDomainAsync(domainId);
}
