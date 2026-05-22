using srv.slots.application.DTOs.Address;
using srv.slots.domain.Entities;

namespace srv.slots.application.Interfaces.Repositories;

public interface ICustomerAddressRepository
{
    Task<List<AddressDto>> GetAllByCustomerAsync(uint customerId);
    Task<AddressDto?> GetByIdAsync(uint id, uint customerId);
    Task<CustomerAddress?> GetEntityByIdAsync(uint id, uint customerId);
    Task<uint> CreateAsync(CustomerAddress address);
    Task<bool> UpdateAsync(CustomerAddress address);
    Task<bool> SoftDeleteAsync(uint id, uint customerId);
    Task ClearDefaultForCustomerAsync(uint customerId);
    Task<bool> SetDefaultAsync(uint id, uint customerId);
}
