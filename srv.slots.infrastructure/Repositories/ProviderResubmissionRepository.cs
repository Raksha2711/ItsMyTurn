using Dapper;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Entities;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class ProviderResubmissionRepository : IProviderResubmissionRepository
{
    private readonly IDbConnectionFactory _factory;
    public ProviderResubmissionRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<ProviderResubmission?> GetLatestPendingAsync(uint providerId)
    {
        const string sql = @"
            SELECT id AS Id, provider_id AS ProviderId, return_reason AS ReturnReason,
                   returned_by AS ReturnedBy, returned_at AS ReturnedAt,
                   resubmitted_at AS ResubmittedAt
            FROM provider_resubmissions
            WHERE provider_id = @Id AND resubmitted_at IS NULL
            ORDER BY id DESC LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<ProviderResubmission>(sql, new { Id = providerId });
    }

    public async Task MarkResubmittedAsync(uint id)
    {
        const string sql = "UPDATE provider_resubmissions SET resubmitted_at = NOW() WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
