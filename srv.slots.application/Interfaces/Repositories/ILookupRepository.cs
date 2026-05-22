using srv.slots.application.DTOs.Public;
using srv.slots.application.DTOs.Provider;

namespace srv.slots.application.Interfaces.Repositories;

public interface ILookupRepository
{
    Task<List<CountryDto>> GetCountriesAsync();
    Task<List<StateDto>> GetStatesAsync(uint countryId);
    Task<List<CityDto>> GetCitiesAsync(uint stateId);
    Task<List<BoundaryDto>> GetBoundariesByCityAsync(uint cityId);

    Task<List<ServiceDomainDto>> GetActiveDomainsAsync();
    Task<List<ServiceCategoryDto>> GetCategoriesByDomainAsync(uint domainId);

    Task<bool> CityExistsAsync(uint cityId);
    Task<bool> DomainExistsAsync(uint domainId);
    Task<bool> CategoryBelongsToDomainAsync(uint categoryId, uint domainId);
    Task<bool> BoundaryBelongsToCityAsync(uint boundaryId, uint cityId);
}
