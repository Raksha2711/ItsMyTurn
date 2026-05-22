using srv.slots.application.DTOs.WorkingHours;

namespace srv.slots.application.Interfaces.Repositories;

public interface IWorkingHoursRepository
{
    Task<List<WorkingHourDayDto>> GetAllByProviderAsync(uint providerId);
    Task UpsertWeekAsync(uint providerId, List<WorkingHourDayDto> days);
    Task UpsertSingleDayAsync(uint providerId, WorkingHourDayDto day);
}
