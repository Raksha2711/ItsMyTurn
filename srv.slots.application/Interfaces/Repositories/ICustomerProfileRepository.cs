using srv.slots.application.DTOs.Profile;
using srv.slots.domain.Entities;

namespace srv.slots.application.Interfaces.Repositories;

public interface ICustomerProfileRepository
{
    Task<CustomerProfileDto?> GetProfileAsync(uint customerId);
    Task<bool> UpdateProfileAsync(uint customerId, UpdateProfileDto dto);
    Task<bool> UpdateFcmTokenAsync(uint customerId, string fcmToken);
    Task<bool> EmailExistsForOtherAsync(string email, uint customerId);
}
