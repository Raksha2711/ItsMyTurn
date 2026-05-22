using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Profile;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class CustomerProfileService : ICustomerProfileService
{
    private readonly ICustomerProfileRepository _repo;

    public CustomerProfileService(ICustomerProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<CustomerProfileDto> GetProfileAsync(uint customerId)
    {
        return await _repo.GetProfileAsync(customerId)
            ?? throw new NotFoundException("Profile not found.");
    }

    public async Task UpdateProfileAsync(uint customerId, UpdateProfileDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var inUse = await _repo.EmailExistsForOtherAsync(dto.Email.Trim().ToLower(), customerId);
            if (inUse) throw new ConflictException("Email already in use by another account.");
        }

        var ok = await _repo.UpdateProfileAsync(customerId, dto);
        if (!ok) throw new NotFoundException("Profile not found.");
    }

    public async Task UpdateFcmTokenAsync(uint customerId, UpdateFcmTokenDto dto)
    {
        var ok = await _repo.UpdateFcmTokenAsync(customerId, dto.FcmToken);
        if (!ok) throw new NotFoundException("Profile not found.");
    }
}
