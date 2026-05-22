using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.WorkingHours;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class WorkingHoursService : IWorkingHoursService
{
    private readonly IWorkingHoursRepository _repo;
    public WorkingHoursService(IWorkingHoursRepository repo) => _repo = repo;

    public Task<List<WorkingHourDayDto>> GetAllAsync(uint providerId)
        => _repo.GetAllByProviderAsync(providerId);

    public async Task UpsertWeekAsync(uint providerId, UpsertWeekDto dto)
    {
        foreach (var d in dto.Days)
        {
            if (d.DayOfWeek > 6) throw new AppException($"Invalid day_of_week: {d.DayOfWeek}");
            if (!d.IsDayOff)
            {
                if (d.OpenTime == null || d.CloseTime == null)
                    throw new AppException($"Day {d.DayOfWeek}: open/close times are required when not a day off.");
                if (d.CloseTime <= d.OpenTime)
                    throw new AppException($"Day {d.DayOfWeek}: close time must be after open time.");
            }
        }

        // Ensure unique days
        var dups = dto.Days.GroupBy(x => x.DayOfWeek).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (dups.Any()) throw new AppException($"Duplicate day_of_week values: {string.Join(",", dups)}");

        await _repo.UpsertWeekAsync(providerId, dto.Days);
    }

    public async Task UpsertDayAsync(uint providerId, WorkingHourDayDto day)
    {
        if (day.DayOfWeek > 6) throw new AppException("Invalid day_of_week.");
        if (!day.IsDayOff)
        {
            if (day.OpenTime == null || day.CloseTime == null)
                throw new AppException("Open/close times required when not a day off.");
            if (day.CloseTime <= day.OpenTime)
                throw new AppException("Close time must be after open time.");
        }
        await _repo.UpsertSingleDayAsync(providerId, day);
    }
}
