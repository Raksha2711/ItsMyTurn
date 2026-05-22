using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using srv.slots.api.Extensions;
using srv.slots.application.DTOs;
using srv.slots.application.DTOs.Address;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.api.Controllers.Customer;

[ApiController]
[Route("api/customer/addresses")]
[Authorize(Roles = "Customer")]
public class AddressController : ControllerBase
{
    private readonly ICustomerAddressService _service;
    public AddressController(ICustomerAddressService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var id = User.GetUserId();
        var list = await _service.GetAllAsync(id);
        return Ok(ApiResponse<List<AddressDto>>.Ok(list));
    }

    [HttpGet("{addressId}")]
    public async Task<IActionResult> Get(uint addressId)
    {
        var id = User.GetUserId();
        var dto = await _service.GetByIdAsync(addressId, id);
        return Ok(ApiResponse<AddressDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertAddressDto dto)
    {
        var id = User.GetUserId();
        var result = await _service.CreateAsync(id, dto);
        return Ok(ApiResponse<AddressDto>.Ok(result, "Address added."));
    }

    [HttpPut("{addressId}")]
    public async Task<IActionResult> Update(uint addressId, [FromBody] UpsertAddressDto dto)
    {
        var id = User.GetUserId();
        var result = await _service.UpdateAsync(addressId, id, dto);
        return Ok(ApiResponse<AddressDto>.Ok(result, "Address updated."));
    }

    [HttpDelete("{addressId}")]
    public async Task<IActionResult> Delete(uint addressId)
    {
        var id = User.GetUserId();
        await _service.DeleteAsync(addressId, id);
        return Ok(ApiResponse.OkMsg("Address deleted."));
    }

    [HttpPost("{addressId}/set-default")]
    public async Task<IActionResult> SetDefault(uint addressId)
    {
        var id = User.GetUserId();
        await _service.SetDefaultAsync(addressId, id);
        return Ok(ApiResponse.OkMsg("Default address updated."));
    }
}
