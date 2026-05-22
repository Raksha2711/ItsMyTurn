using srv.slots.application.DTOs.WorkingHours;

namespace srv.slots.application.Interfaces.Services;

public interface IWorkingHoursService
{
    Task<List<WorkingHourDayDto>> GetAllAsync(uint providerId);
    Task UpsertWeekAsync(uint providerId, UpsertWeekDto dto);
    Task UpsertDayAsync(uint providerId, WorkingHourDayDto dto);
}
