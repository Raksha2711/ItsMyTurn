using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Profile;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Customer;

[ApiController]
[Route("api/customer/profile")]
[Authorize(Roles = "Customer")]
public class ProfileController : ControllerBase
{
    private readonly ICustomerProfileService _service;

    public ProfileController(ICustomerProfileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var customerId = User.GetUserId();
        var dto = await _service.GetProfileAsync(customerId);
        return Ok(ApiResponse<CustomerProfileDto>.Ok(dto));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfileDto dto)
    {
        var customerId = User.GetUserId();
        await _service.UpdateProfileAsync(customerId, dto);
        return Ok(ApiResponse.OkMsg("Profile updated."));
    }

    [HttpPut("fcm-token")]
    public async Task<IActionResult> UpdateFcm([FromBody] UpdateFcmTokenDto dto)
    {
        var customerId = User.GetUserId();
        await _service.UpdateFcmTokenAsync(customerId, dto);
        return Ok(ApiResponse.OkMsg("FCM token updated."));
    }
}
