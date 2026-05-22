using System.ComponentModel.DataAnnotations;
using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.ProviderAuth;

/// <summary>
/// Sent when status = Returned. Provider can update any field admin asked for.
/// All fields optional — only non-null ones get updated.
/// </summary>
public class ProviderResubmitDto
{
    [MaxLength(200)] public string? FirmName { get; set; }
    [MaxLength(150)] public string? OwnerName { get; set; }
    [EmailAddress, MaxLength(191)] public string? Email { get; set; }
    [MaxLength(500)] public string? LogoUrl { get; set; }
    [MaxLength(200)] public string? Specialization { get; set; }
    public string? Description { get; set; }
    public string? FeesStructure { get; set; }
    [MaxLength(300)] public string? Languages { get; set; }
    public string? FullAddress { get; set; }
    public uint? CityId { get; set; }
    [MaxLength(10)] public string? Pincode { get; set; }
    [Range(-90, 90)] public decimal? Latitude { get; set; }
    [Range(-180, 180)] public decimal? Longitude { get; set; }
    public uint? BoundaryId { get; set; }
    public ProviderServiceType? ServiceType { get; set; }
    [Range(5, 480)] public int? AvgServiceDurationMins { get; set; }
    [Range(1, 1000)] public int? MaxCapacityPerDay { get; set; }
    [Range(1, 120)] public int? AvgWaitPerTokenMins { get; set; }
    public uint? DomainId { get; set; }
    public uint? CategoryId { get; set; }
}
