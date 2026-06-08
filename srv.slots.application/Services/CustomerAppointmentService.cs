using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Appointment;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;

namespace srv.slots.application.Services;

public class CustomerAppointmentService : ICustomerAppointmentService
{
    private readonly IAppointmentRepository _repo;
    public CustomerAppointmentService(IAppointmentRepository repo) => _repo = repo;

    public Task<List<CustomerAppointmentDto>> GetMyAppointmentsAsync(uint customerId)
        => _repo.GetByCustomerAsync(customerId);

    public Task<string> BookAsync(uint customerId, BookAppointmentDto dto)
        => _repo.CreateAsync(customerId, dto);

    public async Task CancelAsync(uint id, uint customerId, string? reason)
    {
        var ok = await _repo.CancelByCustomerAsync(id, customerId, reason);
        if (!ok) throw new NotFoundException("Appointment not found or already finalised.");
    }
}
