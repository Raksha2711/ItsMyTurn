using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.WorkingHours;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/working-hours")]
[Authorize(Roles = "Provider")]
public class WorkingHoursController : ControllerBase
{
    private readonly IWorkingHoursService _service;
    public WorkingHoursController(IWorkingHoursService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var list = await _service.GetAllAsync(User.GetUserId());
        return Ok(ApiResponse<List<WorkingHourDayDto>>.Ok(list));
    }

    /// <summary>Bulk update the whole week (Mon–Sun) in one call.</summary>
    [HttpPut]
    public async Task<IActionResult> UpsertWeek([FromBody] UpsertWeekDto dto)
    {
        await _service.UpsertWeekAsync(User.GetUserId(), dto);
        return Ok(ApiResponse.OkMsg("Working hours updated."));
    }

    /// <summary>Update one specific day.</summary>
    [HttpPatch("{dayOfWeek:int}")]
    public async Task<IActionResult> UpsertDay(byte dayOfWeek, [FromBody] WorkingHourDayDto dto)
    {
        dto.DayOfWeek = dayOfWeek;
        await _service.UpsertDayAsync(User.GetUserId(), dto);
        return Ok(ApiResponse.OkMsg("Day updated."));
    }
}
