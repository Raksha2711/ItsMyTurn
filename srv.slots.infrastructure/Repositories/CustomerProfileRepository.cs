using Dapper;
using srv.slots.application.DTOs.Profile;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Enums;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class CustomerProfileRepository : ICustomerProfileRepository
{
    private readonly IDbConnectionFactory _factory;
    public CustomerProfileRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<CustomerProfileDto?> GetProfileAsync(uint customerId)
    {
        const string sql = @"
            SELECT id AS Id, full_name AS FullName, email AS Email, mobile AS Mobile,
                   profile_image AS ProfileImage, date_of_birth AS DateOfBirth,
                   gender AS Gender, is_verified AS IsVerified, created_at AS CreatedAt
            FROM customers
            WHERE id = @Id AND is_terminated = 0 LIMIT 1;";

        using var conn = _factory.CreateConnection();
        // Read gender as string then parse to enum to handle MySQL ENUM correctly
        var row = await conn.QueryFirstOrDefaultAsync(sql, new { Id = customerId });
        if (row == null) return null;

        return new CustomerProfileDto
        {
            Id = (uint)row.Id,
            FullName = row.FullName,
            Email = row.Email,
            Mobile = row.Mobile,
            ProfileImage = row.ProfileImage,
            DateOfBirth = row.DateOfBirth,
            Gender = row.Gender is string g && Enum.TryParse<GenderType>(g, out var p) ? p : (GenderType?)null,
            IsVerified = Convert.ToBoolean(row.IsVerified),
            CreatedAt = row.CreatedAt
        };
    }

    public async Task<bool> UpdateProfileAsync(uint customerId, UpdateProfileDto dto)
    {
        const string sql = @"
            UPDATE customers SET
                full_name = @FullName,
                email = @Email,
                profile_image = @ProfileImage,
                date_of_birth = @DateOfBirth,
                gender = @Gender,
                updated_at = NOW()
            WHERE id = @Id;";

        using var conn = _factory.CreateConnection();
        var rows = await conn.ExecuteAsync(sql, new
        {
            Id = customerId,
            FullName = dto.FullName.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLower(),
            dto.ProfileImage,
            dto.DateOfBirth,
            Gender = dto.Gender?.ToString()
        });
        return rows > 0;
    }

    public async Task<bool> UpdateFcmTokenAsync(uint customerId, string fcmToken)
    {
        const string sql = "UPDATE customers SET fcm_token = @Token WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        var rows = await conn.ExecuteAsync(sql, new { Id = customerId, Token = fcmToken });
        return rows > 0;
    }

    public async Task<bool> EmailExistsForOtherAsync(string email, uint customerId)
    {
        const string sql = "SELECT COUNT(1) FROM customers WHERE email = @Email AND id <> @Id;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Email = email, Id = customerId }) > 0;
    }
}
