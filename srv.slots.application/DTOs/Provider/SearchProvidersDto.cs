using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Provider;

public class SearchProvidersDto
{
    public string? Keyword { get; set; }          // search firm name / specialization
    public uint? DomainId { get; set; }
    public uint? CategoryId { get; set; }
    public uint? SubCategoryId { get; set; }
    public uint? CityId { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    [Range(0.5, 100)]
    public double? RadiusKm { get; set; } = 10;

    [Range(1, 100)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 20;
}
