using srv.slots.domain.Entities;

namespace srv.slots.application.Interfaces.Repositories;

public interface IProviderResubmissionRepository
{
    Task<ProviderResubmission?> GetLatestPendingAsync(uint providerId);
    Task MarkResubmittedAsync(uint id);
}
