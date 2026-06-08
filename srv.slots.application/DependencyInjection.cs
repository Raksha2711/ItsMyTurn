using Microsoft.Extensions.DependencyInjection;
using srv.slots.application.Interfaces.Services;
using srv.slots.application.Services;

namespace srv.slots.application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ── Module 1 ──
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();

        // ── Module 2 ──
        services.AddScoped<ICustomerProfileService, CustomerProfileService>();
        services.AddScoped<ICustomerAddressService, CustomerAddressService>();
        services.AddScoped<IProviderDiscoveryService, ProviderDiscoveryService>();

        // ── Module 3 ──
        services.AddScoped<IProviderAuthService, ProviderAuthService>();
        services.AddScoped<ILookupService, LookupService>();

        // ── Module 4 ──
        services.AddScoped<IProviderServiceManagementService, ProviderServiceManagementService>();
        services.AddScoped<IWorkingHoursService, WorkingHoursService>();
        services.AddScoped<IScheduleOverrideService, ScheduleOverrideService>();
        services.AddScoped<ISlotManagementService, SlotManagementService>();
        services.AddScoped<ITokenConfigService, TokenConfigService>();
        services.AddScoped<IProviderSelfProfileService, ProviderSelfProfileService>();

        // ── Module 5 ──
        services.AddScoped<IProviderAppointmentService, ProviderAppointmentService>();
        services.AddScoped<ICustomerAppointmentService, CustomerAppointmentService>();

        return services;
    }
}
