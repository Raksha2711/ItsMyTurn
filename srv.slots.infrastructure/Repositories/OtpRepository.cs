using Dapper;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Entities;
using srv.slots.domain.Enums;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly IDbConnectionFactory _factory;
    public OtpRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<uint> CreateAsync(CustomerOtp o)
    {
        const string sql = @"
            INSERT INTO customer_otp (mobile, otp_code, purpose, expires_at, is_used, created_at)
            VALUES (@Mobile, @OtpCode, @Purpose, @ExpiresAt, 0, NOW());
            SELECT LAST_INSERT_ID();";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<uint>(sql, new
        {
            o.Mobile,
            o.OtpCode,
            Purpose = o.Purpose.ToString(),
            o.ExpiresAt
        });
    }

    public async Task<CustomerOtp?> GetLatestActiveAsync(string mobile, string otpCode, OtpPurpose purpose)
    {
        const string sql = @"
            SELECT id AS Id, mobile AS Mobile, otp_code AS OtpCode, purpose AS Purpose,
                   expires_at AS ExpiresAt, is_used AS IsUsed, created_at AS CreatedAt
            FROM customer_otp
            WHERE mobile = @Mobile AND otp_code = @OtpCode AND purpose = @Purpose AND is_used = 0
            ORDER BY id DESC LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<CustomerOtp>(sql, new
        {
            Mobile = mobile,
            OtpCode = otpCode,
            Purpose = purpose.ToString()
        });
    }

    public async Task MarkUsedAsync(uint id)
    {
        const string sql = "UPDATE customer_otp SET is_used = 1 WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
