using srv.slots.application.DTOs.ScheduleOverride;

namespace srv.slots.application.Interfaces.Services;

public interface IScheduleOverrideService
{
    Task<List<ScheduleOverrideDto>> GetRangeAsync(uint providerId, DateTime from, DateTime to);
    Task<ScheduleOverrideDto> CreateAsync(uint providerId, UpsertScheduleOverrideDto dto);
    Task<ScheduleOverrideDto> UpdateAsync(uint id, uint providerId, UpsertScheduleOverrideDto dto);
    Task DeleteAsync(uint id, uint providerId);
}
