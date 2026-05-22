using srv.slots.application.DTOs.Address;

namespace srv.slots.application.Interfaces.Services;

public interface ICustomerAddressService
{
    Task<List<AddressDto>> GetAllAsync(uint customerId);
    Task<AddressDto> GetByIdAsync(uint addressId, uint customerId);
    Task<AddressDto> CreateAsync(uint customerId, UpsertAddressDto dto);
    Task<AddressDto> UpdateAsync(uint addressId, uint customerId, UpsertAddressDto dto);
    Task DeleteAsync(uint addressId, uint customerId);
    Task SetDefaultAsync(uint addressId, uint customerId);
}
