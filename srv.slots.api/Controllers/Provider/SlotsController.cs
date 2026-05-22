using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Slot;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/slots")]
[Authorize(Roles = "Provider")]
public class SlotsController : ControllerBase
{
    private readonly ISlotManagementService _service;
    public SlotsController(ISlotManagementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
    {
        var list = await _service.GetByDateAsync(User.GetUserId(), date);
        return Ok(ApiResponse<List<SlotDto>>.Ok(list));
    }

    /// <summary>
    /// Auto-generate slots for a date range based on your weekly working hours
    /// (and respecting any schedule overrides for holidays / special hours).
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateSlotsDto dto)
    {
        var result = await _service.GenerateAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<GenerateSlotsResultDto>.Ok(result, $"Created {result.TotalSlotsCreated} slots."));
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(uint id)
    {
        await _service.ToggleSlotActiveAsync(id, User.GetUserId());
        return Ok(ApiResponse.OkMsg("Slot status toggled."));
    }
}
