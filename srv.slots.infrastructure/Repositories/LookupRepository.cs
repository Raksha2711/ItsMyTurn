using Dapper;
using srv.slots.application.DTOs.Public;
using srv.slots.application.DTOs.Provider;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class LookupRepository : ILookupRepository
{
    private readonly IDbConnectionFactory _factory;
    public LookupRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<List<CountryDto>> GetCountriesAsync()
    {
        const string sql = "SELECT id AS Id, name AS Name, code AS Code FROM countries WHERE is_active = 1 ORDER BY name;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<CountryDto>(sql)).ToList();
    }

    public async Task<List<StateDto>> GetStatesAsync(uint countryId)
    {
        const string sql = @"SELECT id AS Id, country_id AS CountryId, name AS Name, code AS Code
                             FROM states WHERE country_id = @Id AND is_active = 1 ORDER BY name;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<StateDto>(sql, new { Id = countryId })).ToList();
    }

    public async Task<List<CityDto>> GetCitiesAsync(uint stateId)
    {
        const string sql = @"SELECT id AS Id, state_id AS StateId, name AS Name
                             FROM cities WHERE state_id = @Id AND is_active = 1 ORDER BY name;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<CityDto>(sql, new { Id = stateId })).ToList();
    }

    public async Task<List<BoundaryDto>> GetBoundariesByCityAsync(uint cityId)
    {
        const string sql = @"SELECT id AS Id, city_id AS CityId, area_name AS AreaName, pincode AS Pincode,
                                    latitude_center AS LatitudeCenter, longitude_center AS LongitudeCenter,
                                    radius_km AS RadiusKm
                             FROM app_boundaries WHERE city_id = @Id AND is_active = 1 ORDER BY area_name;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<BoundaryDto>(sql, new { Id = cityId })).ToList();
    }

    public async Task<List<ServiceDomainDto>> GetActiveDomainsAsync()
    {
        const string sql = @"SELECT id AS Id, name AS Name, icon AS Icon, sort_order AS SortOrder
                             FROM service_domains WHERE is_active = 1 ORDER BY sort_order, name;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<ServiceDomainDto>(sql)).ToList();
    }

    public async Task<List<ServiceCategoryDto>> GetCategoriesByDomainAsync(uint domainId)
    {
        const string sql = @"SELECT id AS Id, domain_id AS DomainId, name AS Name,
                                    icon AS Icon, description AS Description
                             FROM service_categories WHERE domain_id = @Id AND is_active = 1
                             ORDER BY sort_order, name;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<ServiceCategoryDto>(sql, new { Id = domainId })).ToList();
    }

    public async Task<bool> CityExistsAsync(uint cityId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM cities WHERE id = @Id AND is_active = 1;", new { Id = cityId }) > 0;
    }

    public async Task<bool> DomainExistsAsync(uint domainId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM service_domains WHERE id = @Id AND is_active = 1;", new { Id = domainId }) > 0;
    }

    public async Task<bool> CategoryBelongsToDomainAsync(uint categoryId, uint domainId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM service_categories WHERE id = @CatId AND domain_id = @DomId AND is_active = 1;",
            new { CatId = categoryId, DomId = domainId }) > 0;
    }

    public async Task<bool> BoundaryBelongsToCityAsync(uint boundaryId, uint cityId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM app_boundaries WHERE id = @BId AND city_id = @CId AND is_active = 1;",
            new { BId = boundaryId, CId = cityId }) > 0;
    }
}
