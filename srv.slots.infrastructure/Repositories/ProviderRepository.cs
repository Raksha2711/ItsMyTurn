using System.Text;
using Dapper;
using srv.slots.application.DTOs.Provider;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly IDbConnectionFactory _factory;
    public ProviderRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<PagedResult<ProviderSummaryDto>> SearchAsync(SearchProvidersDto f)
    {
        var where = new StringBuilder(@"
            WHERE p.status = 'Approved'
              AND p.is_active = 1
              AND p.is_terminated = 0 ");

        var p = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(f.Keyword))
        {
            where.Append(" AND (p.firm_name LIKE @Kw OR p.specialization LIKE @Kw OR p.description LIKE @Kw) ");
            p.Add("Kw", $"%{f.Keyword.Trim()}%");
        }
        if (f.DomainId.HasValue) { where.Append(" AND p.domain_id = @DomainId "); p.Add("DomainId", f.DomainId.Value); }
        if (f.CategoryId.HasValue) { where.Append(" AND p.category_id = @CategoryId "); p.Add("CategoryId", f.CategoryId.Value); }
        if (f.SubCategoryId.HasValue)
        {
            where.Append(@" AND EXISTS (SELECT 1 FROM provider_services ps
                            WHERE ps.provider_id = p.id AND ps.sub_category_id = @SubCategoryId AND ps.is_active = 1) ");
            p.Add("SubCategoryId", f.SubCategoryId.Value);
        }
        if (f.CityId.HasValue) { where.Append(" AND p.city_id = @CityId "); p.Add("CityId", f.CityId.Value); }

        // Distance via Haversine formula (in km)
        var hasGeo = f.Latitude.HasValue && f.Longitude.HasValue;
        string distanceExpr = hasGeo
            ? @"(6371 * ACOS(
                    COS(RADIANS(@Lat)) * COS(RADIANS(p.latitude)) *
                    COS(RADIANS(p.longitude) - RADIANS(@Lng)) +
                    SIN(RADIANS(@Lat)) * SIN(RADIANS(p.latitude))
                ))"
            : "NULL";

        if (hasGeo)
        {
            p.Add("Lat", f.Latitude!.Value);
            p.Add("Lng", f.Longitude!.Value);
            where.Append(" AND p.latitude IS NOT NULL AND p.longitude IS NOT NULL ");
            if (f.RadiusKm.HasValue)
            {
                where.Append($" AND {distanceExpr} <= @Radius ");
                p.Add("Radius", f.RadiusKm.Value);
            }
        }

        var offset = (f.Page - 1) * f.PageSize;
        p.Add("Offset", offset);
        p.Add("Limit", f.PageSize);

        string orderBy = hasGeo ? "ORDER BY DistanceKm ASC" : "ORDER BY p.firm_name ASC";

        var sql = $@"
            SELECT
                p.id AS Id, p.firm_name AS FirmName, p.specialization AS Specialization,
                p.logo_url AS LogoUrl, p.domain_id AS DomainId, sd.name AS DomainName,
                p.category_id AS CategoryId, sc.name AS CategoryName,
                p.full_address AS FullAddress, c.name AS CityName, p.pincode AS Pincode,
                p.latitude AS Latitude, p.longitude AS Longitude,
                p.service_type AS ServiceType, p.languages AS Languages,
                {distanceExpr} AS DistanceKm,
                COALESCE(r.avg_rating, 0) AS AvgRating,
                COALESCE(r.review_count, 0) AS ReviewCount
            FROM service_providers p
            LEFT JOIN service_domains sd ON sd.id = p.domain_id
            LEFT JOIN service_categories sc ON sc.id = p.category_id
            LEFT JOIN cities c ON c.id = p.city_id
            LEFT JOIN (
                SELECT provider_id, AVG(rating) AS avg_rating, COUNT(*) AS review_count
                FROM reviews WHERE is_visible = 1 GROUP BY provider_id
            ) r ON r.provider_id = p.id
            {where}
            {orderBy}
            LIMIT @Limit OFFSET @Offset;

            SELECT COUNT(*) FROM service_providers p {where};";

        using var conn = _factory.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(sql, p);
        var items = (await multi.ReadAsync<ProviderSummaryDto>()).ToList();
        var total = await multi.ReadFirstAsync<int>();

        return new PagedResult<ProviderSummaryDto>
        {
            Items = items,
            Page = f.Page,
            PageSize = f.PageSize,
            TotalCount = total
        };
    }

    public async Task<ProviderDetailDto?> GetDetailAsync(uint providerId)
    {
        const string sql = @"
            SELECT
                p.id AS Id, p.firm_name AS FirmName, p.owner_name AS OwnerName,
                p.specialization AS Specialization, p.description AS Description,
                p.logo_url AS LogoUrl, p.fees_structure AS FeesStructure,
                p.languages AS Languages,
                p.domain_id AS DomainId, sd.name AS DomainName,
                p.category_id AS CategoryId, sc.name AS CategoryName,
                p.full_address AS FullAddress, c.name AS CityName, p.pincode AS Pincode,
                p.latitude AS Latitude, p.longitude AS Longitude,
                p.service_type AS ServiceType,
                p.avg_service_duration_mins AS AvgServiceDurationMins,
                p.avg_wait_per_token_mins AS AvgWaitPerTokenMins,
                p.mobile AS Mobile, p.whatsapp_number AS WhatsappNumber,
                COALESCE(r.avg_rating, 0) AS AvgRating,
                COALESCE(r.review_count, 0) AS ReviewCount
            FROM service_providers p
            LEFT JOIN service_domains sd ON sd.id = p.domain_id
            LEFT JOIN service_categories sc ON sc.id = p.category_id
            LEFT JOIN cities c ON c.id = p.city_id
            LEFT JOIN (
                SELECT provider_id, AVG(rating) AS avg_rating, COUNT(*) AS review_count
                FROM reviews WHERE is_visible = 1 GROUP BY provider_id
            ) r ON r.provider_id = p.id
            WHERE p.id = @Id AND p.status = 'Approved' AND p.is_active = 1 AND p.is_terminated = 0
            LIMIT 1;

            SELECT ps.id AS Id, ps.name AS Name, ps.description AS Description,
                   ps.duration_mins AS DurationMins, ps.price AS Price,
                   ps.price_label AS PriceLabel, ps.sub_category_id AS SubCategoryId,
                   ssc.name AS SubCategoryName
            FROM provider_services ps
            LEFT JOIN service_sub_categories ssc ON ssc.id = ps.sub_category_id
            WHERE ps.provider_id = @Id AND ps.is_active = 1;

            SELECT day_of_week AS DayOfWeek, open_time AS OpenTime,
                   close_time AS CloseTime, is_day_off AS IsDayOff
            FROM provider_working_hours
            WHERE provider_id = @Id
            ORDER BY day_of_week;";

        using var conn = _factory.CreateConnection();
        using var multi = await conn.QueryMultipleAsync(sql, new { Id = providerId });
        var detail = await multi.ReadFirstOrDefaultAsync<ProviderDetailDto>();
        if (detail == null) return null;

        detail.Services = (await multi.ReadAsync<ProviderServiceDto>()).ToList();
        var hours = (await multi.ReadAsync<WorkingHourDto>()).ToList();
        foreach (var h in hours)
            h.DayName = ((DayOfWeek)h.DayOfWeek).ToString();
        detail.WorkingHours = hours;

        return detail;
    }
}
