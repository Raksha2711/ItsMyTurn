using Dapper;
using srv.slots.application.DTOs.ProviderProfile;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Enums;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class ProviderSelfProfileRepository : IProviderSelfProfileRepository
{
    private readonly IDbConnectionFactory _factory;
    public ProviderSelfProfileRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<ProviderProfileDto?> GetProfileAsync(uint providerId)
    {
        const string sql = @"
            SELECT p.id AS Id, p.firm_name AS FirmName, p.owner_name AS OwnerName,
                   p.email AS Email, p.mobile AS Mobile, p.whatsapp_number AS WhatsappNumber,
                   p.logo_url AS LogoUrl,
                   p.domain_id AS DomainId, sd.name AS DomainName,
                   p.category_id AS CategoryId, sc.name AS CategoryName,
                   p.specialization AS Specialization, p.description AS Description,
                   p.fees_structure AS FeesStructure, p.languages AS Languages,
                   p.full_address AS FullAddress, p.city_id AS CityId, c.name AS CityName,
                   p.pincode AS Pincode, p.latitude AS Latitude, p.longitude AS Longitude,
                   p.service_type AS ServiceType,
                   p.avg_service_duration_mins AS AvgServiceDurationMins,
                   p.max_capacity_per_day AS MaxCapacityPerDay,
                   p.avg_wait_per_token_mins AS AvgWaitPerTokenMins,
                   p.plan_id AS PlanId, p.status AS Status, p.is_verified AS IsVerified,
                   p.last_login_at AS LastLoginAt, p.created_at AS CreatedAt
            FROM service_providers p
            LEFT JOIN service_domains sd ON sd.id = p.domain_id
            LEFT JOIN service_categories sc ON sc.id = p.category_id
            LEFT JOIN cities c ON c.id = p.city_id
            WHERE p.id = @Id LIMIT 1;";

        using var conn = _factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(sql, new { Id = providerId });
        if (row == null) return null;

        return new ProviderProfileDto
        {
            Id = (uint)row.Id,
            FirmName = row.FirmName,
            OwnerName = row.OwnerName,
            Email = row.Email,
            Mobile = row.Mobile,
            WhatsappNumber = row.WhatsappNumber,
            LogoUrl = row.LogoUrl,
            DomainId = (uint)row.DomainId,
            DomainName = row.DomainName,
            CategoryId = (uint)row.CategoryId,
            CategoryName = row.CategoryName,
            Specialization = row.Specialization,
            Description = row.Description,
            FeesStructure = row.FeesStructure,
            Languages = row.Languages,
            FullAddress = row.FullAddress,
            CityId = (uint)row.CityId,
            CityName = row.CityName,
            Pincode = row.Pincode,
            Latitude = row.Latitude,
            Longitude = row.Longitude,
            ServiceType = Enum.Parse<ProviderServiceType>((string)row.ServiceType),
            AvgServiceDurationMins = (int)row.AvgServiceDurationMins,
            MaxCapacityPerDay = (int)row.MaxCapacityPerDay,
            AvgWaitPerTokenMins = (int)row.AvgWaitPerTokenMins,
            PlanId = (uint)row.PlanId,
            Status = Enum.Parse<ProviderStatus>((string)row.Status),
            IsVerified = Convert.ToBoolean(row.IsVerified),
            LastLoginAt = row.LastLoginAt,
            CreatedAt = row.CreatedAt
        };
    }

    public async Task<bool> UpdateProfileAsync(uint providerId, UpdateProviderProfileDto dto)
    {
        var sets = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Id", providerId);

        void Maybe(string col, string param, object? value)
        {
            if (value == null) return;
            sets.Add($"{col} = @{param}");
            parameters.Add(param, value);
        }

        Maybe("owner_name", "OwnerName", dto.OwnerName?.Trim());
        Maybe("whatsapp_number", "WhatsappNumber", dto.WhatsappNumber);
        Maybe("logo_url", "LogoUrl", dto.LogoUrl);
        Maybe("specialization", "Specialization", dto.Specialization);
        Maybe("description", "Description", dto.Description);
        Maybe("fees_structure", "FeesStructure", dto.FeesStructure);
        Maybe("languages", "Languages", dto.Languages);
        Maybe("full_address", "FullAddress", dto.FullAddress?.Trim());
        Maybe("pincode", "Pincode", dto.Pincode);
        Maybe("latitude", "Latitude", dto.Latitude);
        Maybe("longitude", "Longitude", dto.Longitude);
        Maybe("avg_service_duration_mins", "Dur", dto.AvgServiceDurationMins);
        Maybe("max_capacity_per_day", "Cap", dto.MaxCapacityPerDay);
        Maybe("avg_wait_per_token_mins", "Wait", dto.AvgWaitPerTokenMins);
        if (dto.ServiceType.HasValue)
        {
            sets.Add("service_type = @ServiceType");
            parameters.Add("ServiceType", dto.ServiceType.Value.ToString());
        }

        if (sets.Count == 0) return true;  // nothing to update — treat as success

        sets.Add("updated_at = NOW()");
        var sql = $"UPDATE service_providers SET {string.Join(", ", sets)} WHERE id = @Id;";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, parameters) > 0;
    }
}
