using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.ProviderAuth;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider")]
public class ProviderStatusController : ControllerBase
{
    private readonly IProviderAuthService _auth;

    public ProviderStatusController(IProviderAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>
    /// Provider can check their approval status without logging in.
    /// Pass providerId from signup response (or from JWT if authorized).
    /// </summary>
    [HttpGet("status/{providerId}")]
    public async Task<IActionResult> GetStatus(uint providerId)
    {
        var dto = await _auth.GetStatusAsync(providerId);
        return Ok(ApiResponse<ProviderStatusDto>.Ok(dto));
    }

    /// <summary>
    /// Resubmit application after admin returned it for correction.
    /// Allowed only when status = Returned. Must be a public endpoint
    /// since the provider can't login until approved.
    /// </summary>
    [HttpPost("resubmit/{providerId}")]
    public async Task<IActionResult> Resubmit(uint providerId, [FromBody] ProviderResubmitDto dto)
    {
        await _auth.ResubmitAsync(providerId, dto);
        return Ok(ApiResponse.OkMsg("Application resubmitted. Status changed back to Pending."));
    }
}
