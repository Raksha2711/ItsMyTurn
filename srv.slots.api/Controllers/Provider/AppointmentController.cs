using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Appointment;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/appointments")]
[Authorize(Roles = "Provider")]
public class AppointmentController : ControllerBase
{
    private readonly IProviderAppointmentService _service;
    public AppointmentController(IProviderAppointmentService service) => _service = service;

    /// <summary>Get all appointments for a date (defaults to today).</summary>
    [HttpGet]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime? date)
    {
        var target = (date ?? DateTime.UtcNow).Date;
        var list   = await _service.GetByDateAsync(User.GetUserId(), target);
        return Ok(ApiResponse<List<AppointmentDto>>.Ok(list));
    }

    /// <summary>
    /// Update appointment status.
    /// Valid values: Confirmed | Waiting | Ongoing | Completed | Skipped | Cancelled | Rejected | NoShow
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(uint id, [FromBody] UpdateAppointmentStatusDto dto)
    {
        await _service.UpdateStatusAsync(id, User.GetUserId(), dto);
        return Ok(ApiResponse.OkMsg($"Status updated to {dto.Status}."));
    }

    /// <summary>Today's appointment counts for dashboard stat cards.</summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _service.GetTodayStatsAsync(User.GetUserId());
        return Ok(ApiResponse<AppointmentStatsDto>.Ok(stats));
    }
}
