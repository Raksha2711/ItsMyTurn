using Dapper;
using srv.slots.application.DTOs.TokenConfig;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class TokenConfigRepository : ITokenConfigRepository
{
    private readonly IDbConnectionFactory _factory;
    public TokenConfigRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<TokenConfigDto?> GetByProviderAsync(uint providerId)
    {
        const string sql = @"
            SELECT max_tokens_per_day AS MaxTokensPerDay,
                   avg_wait_per_token_mins AS AvgWaitPerTokenMins,
                   token_hours_start AS TokenHoursStart,
                   token_hours_end AS TokenHoursEnd,
                   auto_reset_daily AS AutoResetDaily,
                   updated_at AS UpdatedAt
            FROM token_queue_config WHERE provider_id = @PId LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<TokenConfigDto>(sql, new { PId = providerId });
    }

    public async Task UpsertAsync(uint providerId, UpsertTokenConfigDto dto)
    {
        const string sql = @"
            INSERT INTO token_queue_config
                (provider_id, max_tokens_per_day, avg_wait_per_token_mins,
                 token_hours_start, token_hours_end, auto_reset_daily, updated_at)
            VALUES
                (@PId, @MaxTokensPerDay, @AvgWaitPerTokenMins,
                 @TokenHoursStart, @TokenHoursEnd, @AutoResetDaily, NOW())
            ON DUPLICATE KEY UPDATE
                max_tokens_per_day      = VALUES(max_tokens_per_day),
                avg_wait_per_token_mins = VALUES(avg_wait_per_token_mins),
                token_hours_start       = VALUES(token_hours_start),
                token_hours_end         = VALUES(token_hours_end),
                auto_reset_daily        = VALUES(auto_reset_daily),
                updated_at              = NOW();";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new
        {
            PId = providerId,
            dto.MaxTokensPerDay,
            dto.AvgWaitPerTokenMins,
            dto.TokenHoursStart,
            dto.TokenHoursEnd,
            dto.AutoResetDaily
        });
    }
}
