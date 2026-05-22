using Dapper;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Entities;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _factory;

    public CustomerRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Customer?> GetByMobileAsync(string mobile)
    {
        const string sql = @"
            SELECT id AS Id, full_name AS FullName, email AS Email, mobile AS Mobile,
                   password_hash AS PasswordHash, profile_image AS ProfileImage,
                   date_of_birth AS DateOfBirth, gender AS Gender,
                   is_verified AS IsVerified, is_active AS IsActive,
                   is_terminated AS IsTerminated, terminated_by AS TerminatedBy,
                   terminated_at AS TerminatedAt, terminated_reason AS TerminatedReason,
                   fcm_token AS FcmToken, last_login_at AS LastLoginAt,
                   created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM customers WHERE mobile = @Mobile LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { Mobile = mobile });
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        const string sql = "SELECT * FROM customers WHERE email = @Email LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = email });
    }

    public async Task<Customer?> GetByIdAsync(uint id)
    {
        const string sql = "SELECT * FROM customers WHERE id = @Id LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
    }

    public async Task<bool> ExistsByMobileAsync(string mobile)
    {
        const string sql = "SELECT COUNT(1) FROM customers WHERE mobile = @Mobile;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Mobile = mobile }) > 0;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        const string sql = "SELECT COUNT(1) FROM customers WHERE email = @Email;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Email = email }) > 0;
    }

    public async Task<uint> CreateAsync(Customer c)
    {
        const string sql = @"
            INSERT INTO customers
                (full_name, email, mobile, password_hash, is_verified, is_active, created_at, updated_at)
            VALUES
                (@FullName, @Email, @Mobile, @PasswordHash, @IsVerified, @IsActive, NOW(), NOW());
            SELECT LAST_INSERT_ID();";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<uint>(sql, c);
    }

    public async Task UpdateLastLoginAsync(uint id)
    {
        const string sql = "UPDATE customers SET last_login_at = NOW() WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task SetVerifiedAsync(uint id)
    {
        const string sql = "UPDATE customers SET is_verified = 1 WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
