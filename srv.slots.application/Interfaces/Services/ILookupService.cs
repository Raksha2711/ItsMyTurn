using srv.slots.application.DTOs.Public;
using srv.slots.application.DTOs.Provider;

namespace srv.slots.application.Interfaces.Services;

public interface ILookupService
{
    Task<List<CountryDto>> GetCountriesAsync();
    Task<List<StateDto>> GetStatesAsync(uint countryId);
    Task<List<CityDto>> GetCitiesAsync(uint stateId);
    Task<List<BoundaryDto>> GetBoundariesAsync(uint cityId);
    Task<List<ServiceDomainDto>> GetDomainsAsync();
    Task<List<ServiceCategoryDto>> GetCategoriesAsync(uint domainId);
}
