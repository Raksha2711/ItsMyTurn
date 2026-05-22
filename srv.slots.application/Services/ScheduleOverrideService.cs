using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.ScheduleOverride;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class ScheduleOverrideService : IScheduleOverrideService
{
    private readonly IScheduleOverrideRepository _repo;
    public ScheduleOverrideService(IScheduleOverrideRepository repo) => _repo = repo;

    public Task<List<ScheduleOverrideDto>> GetRangeAsync(uint providerId, DateTime from, DateTime to)
    {
        if (to < from) throw new AppException("'to' must be on or after 'from'.");
        return _repo.GetByDateRangeAsync(providerId, from.Date, to.Date);
    }

    public async Task<ScheduleOverrideDto> CreateAsync(uint providerId, UpsertScheduleOverrideDto dto)
    {
        Validate(dto);

        // Check if an override already exists for this date
        var existing = await _repo.GetByDateAsync(providerId, dto.OverrideDate.Date);
        if (existing != null)
            throw new ConflictException($"An override already exists for {dto.OverrideDate:yyyy-MM-dd}. Update it instead.");

        var id = await _repo.CreateAsync(providerId, dto);
        return await _repo.GetByIdAsync(id, providerId)
            ?? throw new AppException("Override created but could not be retrieved.");
    }

    public async Task<ScheduleOverrideDto> UpdateAsync(uint id, uint providerId, UpsertScheduleOverrideDto dto)
    {
        Validate(dto);
        var ok = await _repo.UpdateAsync(id, providerId, dto);
        if (!ok) throw new NotFoundException("Override not found.");

        return await _repo.GetByIdAsync(id, providerId)
            ?? throw new NotFoundException("Override not found.");
    }

    public async Task DeleteAsync(uint id, uint providerId)
    {
        var ok = await _repo.DeleteAsync(id, providerId);
        if (!ok) throw new NotFoundException("Override not found.");
    }

    private static void Validate(UpsertScheduleOverrideDto dto)
    {
        if (!dto.IsClosed)
        {
            if (dto.OpenTime == null || dto.CloseTime == null)
                throw new AppException("OpenTime and CloseTime are required when IsClosed is false.");
            if (dto.CloseTime <= dto.OpenTime)
                throw new AppException("CloseTime must be after OpenTime.");
        }
    }
}
