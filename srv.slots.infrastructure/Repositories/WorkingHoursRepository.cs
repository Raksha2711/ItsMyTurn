using Dapper;
using srv.slots.application.DTOs.WorkingHours;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class WorkingHoursRepository : IWorkingHoursRepository
{
    private readonly IDbConnectionFactory _factory;
    public WorkingHoursRepository(IDbConnectionFactory factory) => _factory = factory;

    private static readonly string[] DayNames =
        { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    public async Task<List<WorkingHourDayDto>> GetAllByProviderAsync(uint providerId)
    {
        const string sql = @"
            SELECT day_of_week AS DayOfWeek, open_time AS OpenTime,
                   close_time AS CloseTime, is_day_off AS IsDayOff
            FROM provider_working_hours
            WHERE provider_id = @PId
            ORDER BY day_of_week;";
        using var conn = _factory.CreateConnection();
        var rows = (await conn.QueryAsync<WorkingHourDayDto>(sql, new { PId = providerId })).ToList();
        foreach (var r in rows) r.DayName = DayNames[r.DayOfWeek];
        return rows;
    }

    public async Task UpsertWeekAsync(uint providerId, List<WorkingHourDayDto> days)
    {
        const string sql = @"
            INSERT INTO provider_working_hours
                (provider_id, day_of_week, open_time, close_time, is_day_off, created_at, updated_at)
            VALUES
                (@PId, @DayOfWeek, @OpenTime, @CloseTime, @IsDayOff, NOW(), NOW())
            ON DUPLICATE KEY UPDATE
                open_time  = VALUES(open_time),
                close_time = VALUES(close_time),
                is_day_off = VALUES(is_day_off),
                updated_at = NOW();";

        using var conn = _factory.CreateConnection();
        foreach (var d in days)
        {
            await conn.ExecuteAsync(sql, new
            {
                PId = providerId,
                d.DayOfWeek,
                d.OpenTime,
                d.CloseTime,
                d.IsDayOff
            });
        }
    }

    public async Task UpsertSingleDayAsync(uint providerId, WorkingHourDayDto d)
    {
        const string sql = @"
            INSERT INTO provider_working_hours
                (provider_id, day_of_week, open_time, close_time, is_day_off, created_at, updated_at)
            VALUES
                (@PId, @DayOfWeek, @OpenTime, @CloseTime, @IsDayOff, NOW(), NOW())
            ON DUPLICATE KEY UPDATE
                open_time  = VALUES(open_time),
                close_time = VALUES(close_time),
                is_day_off = VALUES(is_day_off),
                updated_at = NOW();";

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new
        {
            PId = providerId,
            d.DayOfWeek,
            d.OpenTime,
            d.CloseTime,
            d.IsDayOff
        });
    }
}
