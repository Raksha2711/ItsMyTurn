using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.application.Interfaces.Services;
using srv.slots.infrastructure.Persistence;
using srv.slots.infrastructure.Repositories;
using srv.slots.infrastructure.Security;

namespace srv.slots.infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // DB
        services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

        // ── Module 1 ──
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // ── Module 2 ──
        services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProviderRepository, ProviderRepository>();

        // ── Module 3 ──
        services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();
        services.AddScoped<IProviderResubmissionRepository, ProviderResubmissionRepository>();
        services.AddScoped<ILookupRepository, LookupRepository>();

        // ── Module 4 ──
        services.AddScoped<IProviderServiceRepository, ProviderServiceRepository>();
        services.AddScoped<IWorkingHoursRepository, WorkingHoursRepository>();
        services.AddScoped<IScheduleOverrideRepository, ScheduleOverrideRepository>();
        services.AddScoped<ISlotRepository, SlotRepository>();
        services.AddScoped<ITokenConfigRepository, TokenConfigRepository>();
        services.AddScoped<IProviderSelfProfileRepository, ProviderSelfProfileRepository>();

        // ── Module 5 ──
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        // ── Security ──
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
