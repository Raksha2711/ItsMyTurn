using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Address;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;
using srv.slots.domain.Entities;

namespace srv.slots.application.Services;

public class CustomerAddressService : ICustomerAddressService
{
    private readonly ICustomerAddressRepository _repo;

    public CustomerAddressService(ICustomerAddressRepository repo)
    {
        _repo = repo;
    }

    public Task<List<AddressDto>> GetAllAsync(uint customerId)
        => _repo.GetAllByCustomerAsync(customerId);

    public async Task<AddressDto> GetByIdAsync(uint addressId, uint customerId)
    {
        return await _repo.GetByIdAsync(addressId, customerId)
            ?? throw new NotFoundException("Address not found.");
    }

    public async Task<AddressDto> CreateAsync(uint customerId, UpsertAddressDto dto)
    {
        if (dto.IsDefault) await _repo.ClearDefaultForCustomerAsync(customerId);

        var entity = new CustomerAddress
        {
            CustomerId = customerId,
            Label = dto.Label.Trim(),
            FullAddress = dto.FullAddress.Trim(),
            CityId = dto.CityId,
            Pincode = dto.Pincode,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IsDefault = dto.IsDefault,
            IsActive = true
        };

        var id = await _repo.CreateAsync(entity);
        return await GetByIdAsync(id, customerId);
    }

    public async Task<AddressDto> UpdateAsync(uint addressId, uint customerId, UpsertAddressDto dto)
    {
        var existing = await _repo.GetEntityByIdAsync(addressId, customerId)
            ?? throw new NotFoundException("Address not found.");

        if (dto.IsDefault && !existing.IsDefault)
            await _repo.ClearDefaultForCustomerAsync(customerId);

        existing.Label = dto.Label.Trim();
        existing.FullAddress = dto.FullAddress.Trim();
        existing.CityId = dto.CityId;
        existing.Pincode = dto.Pincode;
        existing.Latitude = dto.Latitude;
        existing.Longitude = dto.Longitude;
        existing.IsDefault = dto.IsDefault;

        var ok = await _repo.UpdateAsync(existing);
        if (!ok) throw new AppException("Could not update address.");

        return await GetByIdAsync(addressId, customerId);
    }

    public async Task DeleteAsync(uint addressId, uint customerId)
    {
        var ok = await _repo.SoftDeleteAsync(addressId, customerId);
        if (!ok) throw new NotFoundException("Address not found.");
    }

    public async Task SetDefaultAsync(uint addressId, uint customerId)
    {
        var existing = await _repo.GetEntityByIdAsync(addressId, customerId)
            ?? throw new NotFoundException("Address not found.");

        await _repo.ClearDefaultForCustomerAsync(customerId);
        await _repo.SetDefaultAsync(addressId, customerId);
    }
}
