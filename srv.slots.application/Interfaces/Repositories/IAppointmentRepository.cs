using srv.slots.application.DTOs.Appointment;

namespace srv.slots.application.Interfaces.Repositories;

public interface IAppointmentRepository
{
    // Provider
    Task<List<AppointmentDto>> GetByProviderDateAsync(uint providerId, DateTime date);
    Task<bool>                 UpdateStatusAsync(uint id, uint providerId, UpdateAppointmentStatusDto dto);
    Task<AppointmentStatsDto>  GetTodayStatsAsync(uint providerId);

    // Customer
    Task<List<CustomerAppointmentDto>> GetByCustomerAsync(uint customerId);
    Task<string>                       CreateAsync(uint customerId, BookAppointmentDto dto);
    Task<bool>                         CancelByCustomerAsync(uint id, uint customerId, string? reason);
}
