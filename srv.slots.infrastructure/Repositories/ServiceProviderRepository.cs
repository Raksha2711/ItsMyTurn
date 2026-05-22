using System.Text;
using Dapper;
using srv.slots.application.DTOs.ProviderAuth;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.domain.Entities;
using srv.slots.domain.Enums;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class ServiceProviderRepository : IServiceProviderRepository
{
    private readonly IDbConnectionFactory _factory;
    public ServiceProviderRepository(IDbConnectionFactory factory) => _factory = factory;

    private const string SelectAll = @"
        SELECT id AS Id, firm_name AS FirmName, owner_name AS OwnerName,
               email AS Email, mobile AS Mobile, whatsapp_number AS WhatsappNumber,
               password_hash AS PasswordHash, logo_url AS LogoUrl,
               domain_id AS DomainId, category_id AS CategoryId,
               specialization AS Specialization, description AS Description,
               fees_structure AS FeesStructure, languages AS Languages,
               full_address AS FullAddress, city_id AS CityId, pincode AS Pincode,
               latitude AS Latitude, longitude AS Longitude, boundary_id AS BoundaryId,
               service_type AS ServiceType,
               avg_service_duration_mins AS AvgServiceDurationMins,
               max_capacity_per_day AS MaxCapacityPerDay,
               avg_wait_per_token_mins AS AvgWaitPerTokenMins,
               plan_id AS PlanId, plan_start_date AS PlanStartDate, plan_end_date AS PlanEndDate,
               status AS Status, rejection_reason AS RejectionReason,
               return_reason AS ReturnReason, reviewed_by AS ReviewedBy, reviewed_at AS ReviewedAt,
               is_verified AS IsVerified, is_active AS IsActive,
               is_terminated AS IsTerminated, terminated_by AS TerminatedBy,
               terminated_at AS TerminatedAt, terminated_reason AS TerminatedReason,
               fcm_token AS FcmToken, last_login_at AS LastLoginAt,
               created_at AS CreatedAt, updated_at AS UpdatedAt
        FROM service_providers";

    public async Task<bool> ExistsByMobileAsync(string mobile)
    {
        const string sql = "SELECT COUNT(1) FROM service_providers WHERE mobile = @Mobile;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Mobile = mobile }) > 0;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        const string sql = "SELECT COUNT(1) FROM service_providers WHERE email = @Email;";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Email = email }) > 0;
    }

    public async Task<ServiceProviderEntity?> GetByMobileAsync(string mobile)
    {
        var sql = SelectAll + " WHERE mobile = @Mobile LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await MapAsync(conn, sql, new { Mobile = mobile });
    }

    public async Task<ServiceProviderEntity?> GetByEmailAsync(string email)
    {
        var sql = SelectAll + " WHERE email = @Email LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await MapAsync(conn, sql, new { Email = email });
    }

    public async Task<ServiceProviderEntity?> GetByIdAsync(uint id)
    {
        var sql = SelectAll + " WHERE id = @Id LIMIT 1;";
        using var conn = _factory.CreateConnection();
        return await MapAsync(conn, sql, new { Id = id });
    }

    public async Task<uint> CreateAsync(ServiceProviderEntity e)
    {
        const string sql = @"
            INSERT INTO service_providers
                (firm_name, owner_name, email, mobile, whatsapp_number, password_hash, logo_url,
                 domain_id, category_id, specialization, description, fees_structure, languages,
                 full_address, city_id, pincode, latitude, longitude, boundary_id,
                 service_type, avg_service_duration_mins, max_capacity_per_day, avg_wait_per_token_mins,
                 plan_id, status, is_verified, is_active, is_terminated, created_at, updated_at)
            VALUES
                (@FirmName, @OwnerName, @Email, @Mobile, @WhatsappNumber, @PasswordHash, @LogoUrl,
                 @DomainId, @CategoryId, @Specialization, @Description, @FeesStructure, @Languages,
                 @FullAddress, @CityId, @Pincode, @Latitude, @Longitude, @BoundaryId,
                 @ServiceType, @AvgServiceDurationMins, @MaxCapacityPerDay, @AvgWaitPerTokenMins,
                 @PlanId, @Status, @IsVerified, @IsActive, @IsTerminated, NOW(), NOW());
            SELECT LAST_INSERT_ID();";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<uint>(sql, new
        {
            e.FirmName, e.OwnerName, e.Email, e.Mobile, e.WhatsappNumber, e.PasswordHash, e.LogoUrl,
            e.DomainId, e.CategoryId, e.Specialization, e.Description, e.FeesStructure, e.Languages,
            e.FullAddress, e.CityId, e.Pincode, e.Latitude, e.Longitude, e.BoundaryId,
            ServiceType = e.ServiceType.ToString(),
            e.AvgServiceDurationMins, e.MaxCapacityPerDay, e.AvgWaitPerTokenMins,
            e.PlanId, Status = e.Status.ToString(),
            e.IsVerified, e.IsActive, e.IsTerminated
        });
    }

    public async Task<bool> ResubmitUpdateAsync(uint providerId, ProviderResubmitDto p)
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

        Maybe("firm_name", "FirmName", p.FirmName?.Trim());
        Maybe("owner_name", "OwnerName", p.OwnerName?.Trim());
        Maybe("email", "Email", p.Email?.Trim().ToLower());
        Maybe("logo_url", "LogoUrl", p.LogoUrl);
        Maybe("specialization", "Specialization", p.Specialization);
        Maybe("description", "Description", p.Description);
        Maybe("fees_structure", "FeesStructure", p.FeesStructure);
        Maybe("languages", "Languages", p.Languages);
        Maybe("full_address", "FullAddress", p.FullAddress?.Trim());
        Maybe("city_id", "CityId", p.CityId);
        Maybe("pincode", "Pincode", p.Pincode);
        Maybe("latitude", "Latitude", p.Latitude);
        Maybe("longitude", "Longitude", p.Longitude);
        Maybe("boundary_id", "BoundaryId", p.BoundaryId);
        Maybe("avg_service_duration_mins", "Dur", p.AvgServiceDurationMins);
        Maybe("max_capacity_per_day", "Cap", p.MaxCapacityPerDay);
        Maybe("avg_wait_per_token_mins", "Wait", p.AvgWaitPerTokenMins);
        Maybe("domain_id", "DomainId", p.DomainId);
        Maybe("category_id", "CategoryId", p.CategoryId);
        if (p.ServiceType.HasValue)
        {
            sets.Add("service_type = @ServiceType");
            parameters.Add("ServiceType", p.ServiceType.Value.ToString());
        }

        // Always reset status to Pending + clear return reason
        sets.Add("status = 'Pending'");
        sets.Add("return_reason = NULL");
        sets.Add("updated_at = NOW()");

        var sql = $"UPDATE service_providers SET {string.Join(", ", sets)} WHERE id = @Id;";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, parameters) > 0;
    }

    public async Task UpdateLastLoginAsync(uint id)
    {
        const string sql = "UPDATE service_providers SET last_login_at = NOW() WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateStatusAsync(uint id, ProviderStatus status)
    {
        const string sql = "UPDATE service_providers SET status = @Status, updated_at = NOW() WHERE id = @Id;";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id, Status = status.ToString() });
    }

    public async Task<ProviderStatusDto?> GetStatusAsync(uint providerId)
    {
        const string sql = @"
            SELECT id AS ProviderId, firm_name AS FirmName, status AS Status,
                   is_active AS IsActive, is_terminated AS IsTerminated,
                   rejection_reason AS RejectionReason, return_reason AS ReturnReason,
                   terminated_reason AS TerminatedReason,
                   reviewed_at AS ReviewedAt, created_at AS SubmittedAt
            FROM service_providers WHERE id = @Id LIMIT 1;";

        using var conn = _factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync(sql, new { Id = providerId });
        if (row == null) return null;

        var status = Enum.Parse<ProviderStatus>((string)row.Status);
        var dto = new ProviderStatusDto
        {
            ProviderId = (uint)row.ProviderId,
            FirmName = row.FirmName,
            Status = status,
            IsActive = Convert.ToBoolean(row.IsActive),
            IsTerminated = Convert.ToBoolean(row.IsTerminated),
            RejectionReason = row.RejectionReason,
            ReturnReason = row.ReturnReason,
            TerminatedReason = row.TerminatedReason,
            ReviewedAt = row.ReviewedAt,
            SubmittedAt = row.SubmittedAt
        };

        dto.StatusMessage = dto.IsTerminated
            ? "Account terminated. " + (dto.TerminatedReason ?? "")
            : status switch
            {
                ProviderStatus.Pending => "Application is under review by Super Admin.",
                ProviderStatus.Approved => "Approved. You can log in and start managing services.",
                ProviderStatus.Rejected => "Application rejected. " + (dto.RejectionReason ?? ""),
                ProviderStatus.Returned => "Correction required. " + (dto.ReturnReason ?? "") + " Please resubmit.",
                ProviderStatus.Suspended => "Account suspended. Contact support.",
                ProviderStatus.Terminated => "Account terminated.",
                _ => "Unknown status."
            };
        return dto;
    }

    // ─────────────────────────────────────────────────
    private static async Task<ServiceProviderEntity?> MapAsync(System.Data.IDbConnection conn, string sql, object param)
    {
        var row = await conn.QueryFirstOrDefaultAsync(sql, param);
        if (row == null) return null;

        return new ServiceProviderEntity
        {
            Id = (uint)row.Id,
            FirmName = row.FirmName,
            OwnerName = row.OwnerName,
            Email = row.Email,
            Mobile = row.Mobile,
            WhatsappNumber = row.WhatsappNumber,
            PasswordHash = row.PasswordHash,
            LogoUrl = row.LogoUrl,
            DomainId = (uint)row.DomainId,
            CategoryId = (uint)row.CategoryId,
            Specialization = row.Specialization,
            Description = row.Description,
            FeesStructure = row.FeesStructure,
            Languages = row.Languages,
            FullAddress = row.FullAddress,
            CityId = (uint)row.CityId,
            Pincode = row.Pincode,
            Latitude = row.Latitude,
            Longitude = row.Longitude,
            BoundaryId = row.BoundaryId == null ? null : (uint?)row.BoundaryId,
            ServiceType = Enum.Parse<ProviderServiceType>((string)row.ServiceType),
            AvgServiceDurationMins = (int)row.AvgServiceDurationMins,
            MaxCapacityPerDay = (int)row.MaxCapacityPerDay,
            AvgWaitPerTokenMins = (int)row.AvgWaitPerTokenMins,
            PlanId = (uint)row.PlanId,
            PlanStartDate = row.PlanStartDate,
            PlanEndDate = row.PlanEndDate,
            Status = Enum.Parse<ProviderStatus>((string)row.Status),
            RejectionReason = row.RejectionReason,
            ReturnReason = row.ReturnReason,
            ReviewedBy = row.ReviewedBy == null ? null : (uint?)row.ReviewedBy,
            ReviewedAt = row.ReviewedAt,
            IsVerified = Convert.ToBoolean(row.IsVerified),
            IsActive = Convert.ToBoolean(row.IsActive),
            IsTerminated = Convert.ToBoolean(row.IsTerminated),
            TerminatedBy = row.TerminatedBy == null ? null : (uint?)row.TerminatedBy,
            TerminatedAt = row.TerminatedAt,
            TerminatedReason = row.TerminatedReason,
            FcmToken = row.FcmToken,
            LastLoginAt = row.LastLoginAt,
            CreatedAt = row.CreatedAt,
            UpdatedAt = row.UpdatedAt
        };
    }
}
