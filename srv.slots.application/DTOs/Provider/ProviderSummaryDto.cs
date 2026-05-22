namespace srv.slots.application.DTOs.Provider;

public class ProviderSummaryDto
{
    public uint Id { get; set; }
    public string FirmName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? LogoUrl { get; set; }
    public uint DomainId { get; set; }
    public string? DomainName { get; set; }
    public uint CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string FullAddress { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double? DistanceKm { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string? Languages { get; set; }
    public decimal AvgRating { get; set; }
    public int ReviewCount { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
