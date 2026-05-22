using Dapper;
using srv.slots.application.DTOs.Slot;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Entities;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class SlotRepository : ISlotRepository
{
    private readonly IDbConnectionFactory _factory;
    public SlotRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<List<SlotDto>> GetByDateAsync(uint providerId, DateTime date)
    {
        const string sql = @"
            SELECT s.id AS Id, s.service_id AS ServiceId, ps.name AS ServiceName,
                   s.slot_date AS SlotDate, s.start_time AS StartTime, s.end_time AS EndTime,
                   s.max_bookings AS MaxBookings, s.booked_count AS BookedCount,
                   s.is_blocked AS IsBlocked, s.is_active AS IsActive
            FROM provider_slots s
            LEFT JOIN provider_services ps ON ps.id = s.service_id
            WHERE s.provider_id = @PId AND s.slot_date = @Date
            ORDER BY s.start_time;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<SlotDto>(sql, new { PId = providerId, Date = date })).ToList();
    }

    public async Task<bool> ExistsForDateAsync(uint providerId, DateTime date)
    {
        const string sql = "SELECT COUNT(1) FROM provider_slots WHERE provider_id = @PId AND slot_date = @Date;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { PId = providerId, Date = date }) > 0;
    }

    public async Task<int> BulkCreateAsync(List<ProviderSlot> slots)
    {
        if (!slots.Any()) return 0;

        const string sql = @"
            INSERT INTO provider_slots
                (provider_id, service_id, slot_date, start_time, end_time,
                 max_bookings, booked_count, is_blocked, is_active, created_at)
            VALUES
                (@ProviderId, @ServiceId, @SlotDate, @StartTime, @EndTime,
                 @MaxBookings, @BookedCount, @IsBlocked, @IsActive, NOW());";

        using var conn = _factory.CreateConnection();
        // Dapper supports list parameter for bulk INSERT in one call
        return await conn.ExecuteAsync(sql, slots);
    }

    public async Task<bool> ToggleActiveAsync(uint id, uint providerId)
    {
        const string sql = @"UPDATE provider_slots SET is_active = 1 - is_active
                             WHERE id = @Id AND provider_id = @PId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = id, PId = providerId }) > 0;
    }

    public async Task<SlotDto?> GetByIdAsync(uint id, uint providerId)
    {
        const string sql = @"
            SELECT s.id AS Id, s.service_id AS ServiceId, ps.name AS ServiceName,
                   s.slot_date AS SlotDate, s.start_time AS StartTime, s.end_time AS EndTime,
                   s.max_bookings AS MaxBookings, s.booked_count AS BookedCount,
                   s.is_blocked AS IsBlocked, s.is_active AS IsActive
            FROM provider_slots s
            LEFT JOIN provider_services ps ON ps.id = s.service_id
            WHERE s.id = @Id AND s.provider_id = @PId LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SlotDto>(sql, new { Id = id, PId = providerId });
    }
}
