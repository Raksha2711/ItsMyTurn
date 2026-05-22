using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.ScheduleOverride;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/schedule-overrides")]
[Authorize(Roles = "Provider")]
public class ScheduleOverridesController : ControllerBase
{
    private readonly IScheduleOverrideService _service;
    public ScheduleOverridesController(IScheduleOverrideService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var list = await _service.GetRangeAsync(User.GetUserId(), from, to);
        return Ok(ApiResponse<List<ScheduleOverrideDto>>.Ok(list));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertScheduleOverrideDto dto)
    {
        var result = await _service.CreateAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<ScheduleOverrideDto>.Ok(result, "Override created."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(uint id, [FromBody] UpsertScheduleOverrideDto dto)
    {
        var result = await _service.UpdateAsync(id, User.GetUserId(), dto);
        return Ok(ApiResponse<ScheduleOverrideDto>.Ok(result, "Override updated."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(uint id)
    {
        await _service.DeleteAsync(id, User.GetUserId());
        return Ok(ApiResponse.OkMsg("Override deleted."));
    }
}
