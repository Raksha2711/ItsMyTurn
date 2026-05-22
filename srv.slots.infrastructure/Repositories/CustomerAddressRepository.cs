using Dapper;
using srv.slots.application.DTOs.Address;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Entities;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class CustomerAddressRepository : ICustomerAddressRepository
{
    private readonly IDbConnectionFactory _factory;
    public CustomerAddressRepository(IDbConnectionFactory factory) => _factory = factory;

    private const string SelectDto = @"
        SELECT a.id AS Id, a.label AS Label, a.full_address AS FullAddress,
               a.city_id AS CityId, c.name AS CityName, s.name AS StateName,
               a.pincode AS Pincode, a.latitude AS Latitude, a.longitude AS Longitude,
               a.is_default AS IsDefault, a.created_at AS CreatedAt
        FROM customer_addresses a
        LEFT JOIN cities c ON c.id = a.city_id
        LEFT JOIN states s ON s.id = c.state_id";

    public async Task<List<AddressDto>> GetAllByCustomerAsync(uint customerId)
    {
        string sql = SelectDto + @"
            WHERE a.customer_id = @CustomerId AND a.is_active = 1
            ORDER BY a.is_default DESC, a.created_at DESC;";

        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync<AddressDto>(sql, new { CustomerId = customerId });
        return rows.ToList();
    }

    public async Task<AddressDto?> GetByIdAsync(uint id, uint customerId)
    {
        string sql = SelectDto + @"
            WHERE a.id = @Id AND a.customer_id = @CustomerId AND a.is_active = 1 LIMIT 1;";

        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<AddressDto>(sql, new { Id = id, CustomerId = customerId });
    }

    public async Task<CustomerAddress?> GetEntityByIdAsync(uint id, uint customerId)
    {
        const string sql = @"
            SELECT id AS Id, customer_id AS CustomerId, label AS Label,
                   full_address AS FullAddress, city_id AS CityId, pincode AS Pincode,
                   latitude AS Latitude, longitude AS Longitude,
                   is_default AS IsDefault, is_active AS IsActive, created_at AS CreatedAt
            FROM customer_addresses
            WHERE id = @Id AND customer_id = @CustomerId AND is_active = 1 LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<CustomerAddress>(sql, new { Id = id, CustomerId = customerId });
    }

    public async Task<uint> CreateAsync(CustomerAddress a)
    {
        const string sql = @"
            INSERT INTO customer_addresses
                (customer_id, label, full_address, city_id, pincode, latitude, longitude, is_default, is_active, created_at)
            VALUES
                (@CustomerId, @Label, @FullAddress, @CityId, @Pincode, @Latitude, @Longitude, @IsDefault, 1, NOW());
            SELECT LAST_INSERT_ID();";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<uint>(sql, a);
    }

    public async Task<bool> UpdateAsync(CustomerAddress a)
    {
        const string sql = @"
            UPDATE customer_addresses SET
                label = @Label, full_address = @FullAddress, city_id = @CityId,
                pincode = @Pincode, latitude = @Latitude, longitude = @Longitude,
                is_default = @IsDefault
            WHERE id = @Id AND customer_id = @CustomerId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, a) > 0;
    }

    public async Task<bool> SoftDeleteAsync(uint id, uint customerId)
    {
        const string sql = "UPDATE customer_addresses SET is_active = 0 WHERE id = @Id AND customer_id = @CustomerId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = id, CustomerId = customerId }) > 0;
    }

    public async Task ClearDefaultForCustomerAsync(uint customerId)
    {
        const string sql = "UPDATE customer_addresses SET is_default = 0 WHERE customer_id = @CustomerId;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { CustomerId = customerId });
    }

    public async Task<bool> SetDefaultAsync(uint id, uint customerId)
    {
        const string sql = "UPDATE customer_addresses SET is_default = 1 WHERE id = @Id AND customer_id = @CustomerId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = id, CustomerId = customerId }) > 0;
    }
}
