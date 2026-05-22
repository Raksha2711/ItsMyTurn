using srv.slots.application.DTOs.Profile;

namespace srv.slots.application.Interfaces.Services;

public interface ICustomerProfileService
{
    Task<CustomerProfileDto> GetProfileAsync(uint customerId);
    Task UpdateProfileAsync(uint customerId, UpdateProfileDto dto);
    Task UpdateFcmTokenAsync(uint customerId, UpdateFcmTokenDto dto);
}
