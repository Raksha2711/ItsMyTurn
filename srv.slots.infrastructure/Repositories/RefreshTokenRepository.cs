using Dapper;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Entities;
using srv.slots.domain.Enums;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _factory;
    public RefreshTokenRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<ulong> CreateAsync(RefreshToken t)
    {
        const string sql = @"
            INSERT INTO refresh_tokens
                (user_type, user_id, token_hash, device_info, ip_address, expires_at, created_at)
            VALUES
                (@UserType, @UserId, @TokenHash, @DeviceInfo, @IpAddress, @ExpiresAt, NOW());
            SELECT LAST_INSERT_ID();";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<ulong>(sql, new
        {
            UserType = t.UserType.ToString(),
            t.UserId,
            t.TokenHash,
            t.DeviceInfo,
            t.IpAddress,
            t.ExpiresAt
        });
    }

    public async Task<RefreshToken?> GetActiveByHashAsync(string tokenHash)
    {
        const string sql = @"
            SELECT id AS Id, user_type AS UserType, user_id AS UserId, token_hash AS TokenHash,
                   device_info AS DeviceInfo, ip_address AS IpAddress,
                   expires_at AS ExpiresAt, revoked_at AS RevokedAt, created_at AS CreatedAt
            FROM refresh_tokens
            WHERE token_hash = @Hash AND revoked_at IS NULL LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { Hash = tokenHash });
    }

    public async Task RevokeAsync(ulong id)
    {
        const string sql = "UPDATE refresh_tokens SET revoked_at = NOW() WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task RevokeAllForUserAsync(UserType userType, uint userId)
    {
        const string sql = @"UPDATE refresh_tokens SET revoked_at = NOW()
            WHERE user_type = @UserType AND user_id = @UserId AND revoked_at IS NULL;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { UserType = userType.ToString(), UserId = userId });
    }
}
