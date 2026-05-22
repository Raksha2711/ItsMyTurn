using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.TokenConfig;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Provider;

[ApiController]
[Route("api/provider/token-config")]
[Authorize(Roles = "Provider")]
public class TokenConfigController : ControllerBase
{
    private readonly ITokenConfigService _service;
    public TokenConfigController(ITokenConfigService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dto = await _service.GetAsync(User.GetUserId());
        return Ok(ApiResponse<TokenConfigDto>.Ok(dto));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertTokenConfigDto dto)
    {
        await _service.UpsertAsync(User.GetUserId(), dto);
        return Ok(ApiResponse.OkMsg("Token configuration saved."));
    }
}
