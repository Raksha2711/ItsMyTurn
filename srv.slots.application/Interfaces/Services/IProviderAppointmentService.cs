using srv.slots.application.DTOs.Appointment;

namespace srv.slots.application.Interfaces.Services;

public interface IProviderAppointmentService
{
    Task<List<AppointmentDto>> GetByDateAsync(uint providerId, DateTime date);
    Task                       UpdateStatusAsync(uint id, uint providerId, UpdateAppointmentStatusDto dto);
    Task<AppointmentStatsDto>  GetTodayStatsAsync(uint providerId);
}
