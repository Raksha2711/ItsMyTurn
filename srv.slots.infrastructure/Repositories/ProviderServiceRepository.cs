using Dapper;
using srv.slots.application.DTOs.ProviderService;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class ProviderServiceRepository : IProviderServiceRepository
{
    private readonly IDbConnectionFactory _factory;
    public ProviderServiceRepository(IDbConnectionFactory factory) => _factory = factory;

    private const string SelectDto = @"
        SELECT ps.id AS Id, ps.sub_category_id AS SubCategoryId,
               ssc.name AS SubCategoryName,
               ps.name AS Name, ps.description AS Description,
               ps.duration_mins AS DurationMins, ps.price AS Price,
               ps.price_label AS PriceLabel, ps.is_active AS IsActive,
               ps.created_at AS CreatedAt
        FROM provider_services ps
        LEFT JOIN service_sub_categories ssc ON ssc.id = ps.sub_category_id";

    public async Task<List<ProviderServiceDto>> GetAllByProviderAsync(uint providerId)
    {
        var sql = SelectDto + " WHERE ps.provider_id = @PId ORDER BY ps.created_at DESC;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<ProviderServiceDto>(sql, new { PId = providerId })).ToList();
    }

    public async Task<ProviderServiceDto?> GetByIdAsync(uint id, uint providerId)
    {
        var sql = SelectDto + " WHERE ps.id = @Id AND ps.provider_id = @PId LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<ProviderServiceDto>(sql, new { Id = id, PId = providerId });
    }

    public async Task<bool> ExistsForProviderAsync(uint id, uint providerId)
    {
        const string sql = "SELECT COUNT(1) FROM provider_services WHERE id = @Id AND provider_id = @PId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Id = id, PId = providerId }) > 0;
    }

    public async Task<uint> CreateAsync(uint providerId, UpsertProviderServiceDto dto)
    {
        const string sql = @"
            INSERT INTO provider_services
                (provider_id, sub_category_id, name, description, duration_mins, price, price_label, is_active, created_at, updated_at)
            VALUES
                (@PId, @SubCategoryId, @Name, @Description, @DurationMins, @Price, @PriceLabel, 1, NOW(), NOW());
            SELECT LAST_INSERT_ID();";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<uint>(sql, new
        {
            PId = providerId,
            dto.SubCategoryId,
            Name = dto.Name.Trim(),
            dto.Description,
            dto.DurationMins,
            dto.Price,
            dto.PriceLabel
        });
    }

    public async Task<bool> UpdateAsync(uint id, uint providerId, UpsertProviderServiceDto dto)
    {
        const string sql = @"
            UPDATE provider_services SET
                sub_category_id = @SubCategoryId, name = @Name, description = @Description,
                duration_mins = @DurationMins, price = @Price, price_label = @PriceLabel,
                updated_at = NOW()
            WHERE id = @Id AND provider_id = @PId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new
        {
            Id = id,
            PId = providerId,
            dto.SubCategoryId,
            Name = dto.Name.Trim(),
            dto.Description,
            dto.DurationMins,
            dto.Price,
            dto.PriceLabel
        }) > 0;
    }

    public async Task<bool> SoftDeleteAsync(uint id, uint providerId)
    {
        const string sql = "UPDATE provider_services SET is_active = 0 WHERE id = @Id AND provider_id = @PId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = id, PId = providerId }) > 0;
    }

    public async Task<bool> ToggleActiveAsync(uint id, uint providerId)
    {
        const string sql = @"UPDATE provider_services SET is_active = 1 - is_active
                             WHERE id = @Id AND provider_id = @PId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = id, PId = providerId }) > 0;
    }
}
