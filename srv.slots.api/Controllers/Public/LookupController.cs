using Microsoft.AspNetCore.Mvc;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Public;
using srv.slots.application.DTOs.Provider;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Public;

[ApiController]
[Route("api/public")]
public class LookupController : ControllerBase
{
    private readonly ILookupService _service;
    public LookupController(ILookupService service) => _service = service;

    [HttpGet("countries")]
    public async Task<IActionResult> Countries()
    {
        var list = await _service.GetCountriesAsync();
        return Ok(ApiResponse<List<CountryDto>>.Ok(list));
    }

    [HttpGet("states")]
    public async Task<IActionResult> States([FromQuery] uint countryId)
    {
        var list = await _service.GetStatesAsync(countryId);
        return Ok(ApiResponse<List<StateDto>>.Ok(list));
    }

    [HttpGet("cities")]
    public async Task<IActionResult> Cities([FromQuery] uint stateId)
    {
        var list = await _service.GetCitiesAsync(stateId);
        return Ok(ApiResponse<List<CityDto>>.Ok(list));
    }

    [HttpGet("boundaries")]
    public async Task<IActionResult> Boundaries([FromQuery] uint cityId)
    {
        var list = await _service.GetBoundariesAsync(cityId);
        return Ok(ApiResponse<List<BoundaryDto>>.Ok(list));
    }

    [HttpGet("domains")]
    public async Task<IActionResult> Domains()
    {
        var list = await _service.GetDomainsAsync();
        return Ok(ApiResponse<List<ServiceDomainDto>>.Ok(list));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> Categories([FromQuery] uint domainId)
    {
        var list = await _service.GetCategoriesAsync(domainId);
        return Ok(ApiResponse<List<ServiceCategoryDto>>.Ok(list));
    }
}
