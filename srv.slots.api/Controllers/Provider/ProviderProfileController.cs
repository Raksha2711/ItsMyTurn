using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.ProviderProfile;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/profile")]
[Authorize(Roles = "Provider")]
public class ProviderProfileController : ControllerBase
{
    private readonly IProviderSelfProfileService _service;
    public ProviderProfileController(IProviderSelfProfileService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dto = await _service.GetAsync(User.GetUserId());
        return Ok(ApiResponse<ProviderProfileDto>.Ok(dto));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProviderProfileDto dto)
    {
        await _service.UpdateAsync(User.GetUserId(), dto);
        return Ok(ApiResponse.OkMsg("Profile updated."));
    }
}
