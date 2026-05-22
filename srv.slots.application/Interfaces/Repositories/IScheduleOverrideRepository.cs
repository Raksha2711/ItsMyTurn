using srv.slots.application.DTOs.ScheduleOverride;

namespace srv.slots.application.Interfaces.Repositories;

public interface IScheduleOverrideRepository
{
    Task<List<ScheduleOverrideDto>> GetByDateRangeAsync(uint providerId, DateTime from, DateTime to);
    Task<ScheduleOverrideDto?> GetByIdAsync(uint id, uint providerId);
    Task<ScheduleOverrideDto?> GetByDateAsync(uint providerId, DateTime date);
    Task<uint> CreateAsync(uint providerId, UpsertScheduleOverrideDto dto);
    Task<bool> UpdateAsync(uint id, uint providerId, UpsertScheduleOverrideDto dto);
    Task<bool> DeleteAsync(uint id, uint providerId);
}
