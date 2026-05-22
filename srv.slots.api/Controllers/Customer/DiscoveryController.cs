using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Provider;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Customer;

[ApiController]
[Route("api/customer/discovery")]
[Authorize(Roles = "Customer")]
public class DiscoveryController : ControllerBase
{
    private readonly IProviderDiscoveryService _service;

    public DiscoveryController(IProviderDiscoveryService service)
    {
        _service = service;
    }

    /// <summary>Full hierarchy of Domains → Categories → SubCategories. Useful for filter UI.</summary>
    [HttpGet("categories")]
    [AllowAnonymous]  // category list is public
    public async Task<IActionResult> Categories()
    {
        var tree = await _service.GetCategoryTreeAsync();
        return Ok(ApiResponse<List<ServiceDomainDto>>.Ok(tree));
    }

    [HttpGet("providers")]
    public async Task<IActionResult> Search([FromQuery] SearchProvidersDto filter)
    {
        var result = await _service.SearchAsync(filter);
        return Ok(ApiResponse<PagedResult<ProviderSummaryDto>>.Ok(result));
    }

    [HttpGet("providers/{providerId}")]
    public async Task<IActionResult> Detail(uint providerId)
    {
        var detail = await _service.GetDetailAsync(providerId);
        return Ok(ApiResponse<ProviderDetailDto>.Ok(detail));
    }
}
