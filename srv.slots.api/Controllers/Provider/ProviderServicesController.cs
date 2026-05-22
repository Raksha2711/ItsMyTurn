using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.ProviderService;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/services")]
[Authorize(Roles = "Provider")]
public class ProviderServicesController : ControllerBase
{
    private readonly IProviderServiceManagementService _service;
    public ProviderServicesController(IProviderServiceManagementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync(User.GetUserId());
        return Ok(ApiResponse<List<ProviderServiceDto>>.Ok(list));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(uint id)
    {
        var dto = await _service.GetByIdAsync(id, User.GetUserId());
        return Ok(ApiResponse<ProviderServiceDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertProviderServiceDto dto)
    {
        var result = await _service.CreateAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<ProviderServiceDto>.Ok(result, "Service created."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(uint id, [FromBody] UpsertProviderServiceDto dto)
    {
        var result = await _service.UpdateAsync(id, User.GetUserId(), dto);
        return Ok(ApiResponse<ProviderServiceDto>.Ok(result, "Service updated."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(uint id)
    {
        await _service.DeleteAsync(id, User.GetUserId());
        return Ok(ApiResponse.OkMsg("Service deleted."));
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(uint id)
    {
        await _service.ToggleActiveAsync(id, User.GetUserId());
        return Ok(ApiResponse.OkMsg("Service status toggled."));
    }
}
