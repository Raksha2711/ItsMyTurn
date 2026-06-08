using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Appointment;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class ProviderAppointmentService : IProviderAppointmentService
{
    private readonly IAppointmentRepository _repo;
    public ProviderAppointmentService(IAppointmentRepository repo) => _repo = repo;

    public Task<List<AppointmentDto>> GetByDateAsync(uint providerId, DateTime date)
        => _repo.GetByProviderDateAsync(providerId, date.Date);

    public async Task UpdateStatusAsync(uint id, uint providerId, UpdateAppointmentStatusDto dto)
    {
        var valid = new[] { "Confirmed", "Waiting", "Ongoing", "Completed", "Skipped", "Cancelled", "Rejected", "NoShow" };
        if (!valid.Contains(dto.Status))
            throw new AppException($"Invalid status. Allowed: {string.Join(", ", valid)}");

        var ok = await _repo.UpdateStatusAsync(id, providerId, dto);
        if (!ok) throw new NotFoundException("Appointment not found.");
    }

    public Task<AppointmentStatsDto> GetTodayStatsAsync(uint providerId)
        => _repo.GetTodayStatsAsync(providerId);
}
