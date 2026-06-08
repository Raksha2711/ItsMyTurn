using srv.slots.application.DTOs.Appointment;

namespace srv.slots.application.Interfaces.Services;

public interface ICustomerAppointmentService
{
    Task<List<CustomerAppointmentDto>> GetMyAppointmentsAsync(uint customerId);
    Task<string>                       BookAsync(uint customerId, BookAppointmentDto dto);
    Task                               CancelAsync(uint id, uint customerId, string? reason);
}
