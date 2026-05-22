using srv.slots.application.DTOs.ProviderAuth;
using srv.slots.domain.Entities;
using srv.slots.domain.Enums;

namespace srv.slots.application.Interfaces.Repositories;

public interface IServiceProviderRepository
{
    Task<bool> ExistsByMobileAsync(string mobile);
    Task<bool> ExistsByEmailAsync(string email);

    Task<ServiceProviderEntity?> GetByMobileAsync(string mobile);
    Task<ServiceProviderEntity?> GetByEmailAsync(string email);
    Task<ServiceProviderEntity?> GetByIdAsync(uint id);

    Task<uint> CreateAsync(ServiceProviderEntity entity);

    /// <summary>Used during resubmission. Only updates supplied non-null fields. Sets status back to Pending.</summary>
    Task<bool> ResubmitUpdateAsync(uint providerId, ProviderResubmitDto patch);

    Task UpdateLastLoginAsync(uint id);
    Task UpdateStatusAsync(uint id, ProviderStatus status);

    Task<ProviderStatusDto?> GetStatusAsync(uint providerId);
}
