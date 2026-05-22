using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Slot;
using srv.slots.application.DTOs.WorkingHours;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;
using srv.slots.domain.Entities;

namespace srv.slots.application.Services;

public class SlotManagementService : ISlotManagementService
{
    private readonly ISlotRepository _slotRepo;
    private readonly IWorkingHoursRepository _whRepo;
    private readonly IScheduleOverrideRepository _overrideRepo;

    public SlotManagementService(
        ISlotRepository slotRepo,
        IWorkingHoursRepository whRepo,
        IScheduleOverrideRepository overrideRepo)
    {
        _slotRepo = slotRepo;
        _whRepo = whRepo;
        _overrideRepo = overrideRepo;
    }

    public Task<List<SlotDto>> GetByDateAsync(uint providerId, DateTime date)
        => _slotRepo.GetByDateAsync(providerId, date.Date);

    public async Task<GenerateSlotsResultDto> GenerateAsync(uint providerId, GenerateSlotsDto dto)
    {
        // 1. Validation
        if (dto.ToDate < dto.FromDate)
            throw new AppException("ToDate must be on or after FromDate.");

        var rangeDays = (dto.ToDate.Date - dto.FromDate.Date).Days;
        if (rangeDays > 90)
            throw new AppException("You can generate slots for up to 90 days at a time.");

        if (dto.FromDate.Date < DateTime.UtcNow.Date.AddDays(-1))
            throw new AppException("Cannot generate slots in the past.");

        // 2. Fetch working hours (one query)
        var weeklyHours = await _whRepo.GetAllByProviderAsync(providerId);
        if (!weeklyHours.Any())
            throw new AppException("Set up your weekly working hours before generating slots.");

        var hoursByDay = weeklyHours.ToDictionary(w => w.DayOfWeek);

        // 3. Fetch overrides for the range
        var overrides = await _overrideRepo.GetByDateRangeAsync(providerId, dto.FromDate.Date, dto.ToDate.Date);
        var overridesByDate = overrides.ToDictionary(o => o.OverrideDate.Date);

        // 4. Build slot list
        var newSlots = new List<ProviderSlot>();
        var result = new GenerateSlotsResultDto();

        for (var d = dto.FromDate.Date; d <= dto.ToDate.Date; d = d.AddDays(1))
        {
            // Skip if slots already exist for this date
            var alreadyHas = await _slotRepo.ExistsForDateAsync(providerId, d);
            if (alreadyHas)
            {
                if (dto.SkipExisting)
                {
                    result.DatesSkipped++;
                    result.Notes.Add($"{d:yyyy-MM-dd}: skipped (slots already exist)");
                    continue;
                }
                throw new ConflictException($"Slots already exist for {d:yyyy-MM-dd}. Use SkipExisting=true to bypass.");
            }

            // Determine open/close for this date
            TimeSpan? open = null, close = null;

            if (overridesByDate.TryGetValue(d.Date, out var ov))
            {
                if (ov.IsClosed)
                {
                    result.DatesSkipped++;
                    result.Notes.Add($"{d:yyyy-MM-dd}: skipped (closed via override: {ov.Note ?? "no note"})");
                    continue;
                }
                open = ov.OpenTime;
                close = ov.CloseTime;
            }
            else
            {
                var dayKey = (byte)d.DayOfWeek;   // 0=Sun..6=Sat
                if (!hoursByDay.TryGetValue(dayKey, out var wh) || wh.IsDayOff)
                {
                    result.DatesSkipped++;
                    result.Notes.Add($"{d:yyyy-MM-dd}: skipped (day off)");
                    continue;
                }
                open = wh.OpenTime;
                close = wh.CloseTime;
            }

            if (open == null || close == null) continue;

            // Generate slot times for the day
            var cursor = open.Value;
            while (cursor.Add(TimeSpan.FromMinutes(dto.SlotDurationMins)) <= close.Value)
            {
                var slotEnd = cursor.Add(TimeSpan.FromMinutes(dto.SlotDurationMins));
                newSlots.Add(new ProviderSlot
                {
                    ProviderId = providerId,
                    ServiceId = dto.ServiceId,
                    SlotDate = d,
                    StartTime = cursor,
                    EndTime = slotEnd,
                    MaxBookings = dto.MaxBookingsPerSlot,
                    BookedCount = 0,
                    IsBlocked = false,
                    IsActive = true
                });
                cursor = slotEnd;
            }

            result.DatesProcessed++;
        }

        // 5. Bulk insert
        result.TotalSlotsCreated = await _slotRepo.BulkCreateAsync(newSlots);
        return result;
    }

    public async Task ToggleSlotActiveAsync(uint slotId, uint providerId)
    {
        var ok = await _slotRepo.ToggleActiveAsync(slotId, providerId);
        if (!ok) throw new NotFoundException("Slot not found.");
    }
}
