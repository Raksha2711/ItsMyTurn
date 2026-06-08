using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Appointment;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Customer;

[ApiController]
[Route("api/customer/appointments")]
[Authorize(Roles = "Customer")]
public class CustomerAppointmentController : ControllerBase
{
    private readonly ICustomerAppointmentService _service;
    public CustomerAppointmentController(ICustomerAppointmentService service) => _service = service;

    /// <summary>List all appointments for the logged-in customer.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var list = await _service.GetMyAppointmentsAsync(User.GetUserId());
        return Ok(ApiResponse<List<CustomerAppointmentDto>>.Ok(list));
    }

    /// <summary>
    /// Book a slot. Returns the booking reference (e.g. #BK00001).
    /// Body: { slotId, patientName?, patientAge?, visitReason? }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Book([FromBody] BookAppointmentDto dto)
    {
        var bookingRef = await _service.BookAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<object>.Ok(
            new { bookingRef },
            $"Appointment booked. Your reference is {bookingRef}."));
    }

    /// <summary>Cancel a Pending, Confirmed, Waiting, or Ongoing appointment.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(uint id, [FromQuery] string? reason)
    {
        await _service.CancelAsync(id, User.GetUserId(), reason);
        return Ok(ApiResponse.OkMsg("Appointment cancelled."));
    }
}
