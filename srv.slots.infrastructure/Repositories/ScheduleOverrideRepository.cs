using Dapper;
using srv.slots.application.DTOs.ScheduleOverride;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class ScheduleOverrideRepository : IScheduleOverrideRepository
{
    private readonly IDbConnectionFactory _factory;
    public ScheduleOverrideRepository(IDbConnectionFactory factory) => _factory = factory;

    private const string SelectDto = @"
        SELECT id AS Id, override_date AS OverrideDate, is_closed AS IsClosed,
               open_time AS OpenTime, close_time AS CloseTime,
               note AS Note, created_at AS CreatedAt
        FROM provider_schedule_overrides";

    public async Task<List<ScheduleOverrideDto>> GetByDateRangeAsync(uint providerId, DateTime from, DateTime to)
    {
        var sql = SelectDto + @" WHERE provider_id = @PId AND override_date BETWEEN @From AND @To
                                 ORDER BY override_date;";
        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<ScheduleOverrideDto>(sql, new { PId = providerId, From = from, To = to })).ToList();
    }

    public async Task<ScheduleOverrideDto?> GetByIdAsync(uint id, uint providerId)
    {
        var sql = SelectDto + " WHERE id = @Id AND provider_id = @PId LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<ScheduleOverrideDto>(sql, new { Id = id, PId = providerId });
    }

    public async Task<ScheduleOverrideDto?> GetByDateAsync(uint providerId, DateTime date)
    {
        var sql = SelectDto + " WHERE provider_id = @PId AND override_date = @Date LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<ScheduleOverrideDto>(sql, new { PId = providerId, Date = date });
    }

    public async Task<uint> CreateAsync(uint providerId, UpsertScheduleOverrideDto dto)
    {
        const string sql = @"
            INSERT INTO provider_schedule_overrides
                (provider_id, override_date, is_closed, open_time, close_time, note, created_at)
            VALUES (@PId, @OverrideDate, @IsClosed, @OpenTime, @CloseTime, @Note, NOW());
            SELECT LAST_INSERT_ID();";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<uint>(sql, new
        {
            PId = providerId,
            OverrideDate = dto.OverrideDate.Date,
            dto.IsClosed,
            dto.OpenTime,
            dto.CloseTime,
            dto.Note
        });
    }

    public async Task<bool> UpdateAsync(uint id, uint providerId, UpsertScheduleOverrideDto dto)
    {
        const string sql = @"
            UPDATE provider_schedule_overrides SET
                override_date = @OverrideDate, is_closed = @IsClosed,
                open_time = @OpenTime, close_time = @CloseTime, note = @Note
            WHERE id = @Id AND provider_id = @PId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new
        {
            Id = id,
            PId = providerId,
            OverrideDate = dto.OverrideDate.Date,
            dto.IsClosed,
            dto.OpenTime,
            dto.CloseTime,
            dto.Note
        }) > 0;
    }

    public async Task<bool> DeleteAsync(uint id, uint providerId)
    {
        const string sql = "DELETE FROM provider_schedule_overrides WHERE id = @Id AND provider_id = @PId;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new { Id = id, PId = providerId }) > 0;
    }
}
