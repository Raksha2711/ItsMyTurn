using System.ComponentModel.DataAnnotations;
using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.ProviderProfile;

public class ProviderProfileDto
{
    public uint Id { get; set; }
    public string FirmName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? WhatsappNumber { get; set; }
    public string? LogoUrl { get; set; }
    public uint DomainId { get; set; }
    public string? DomainName { get; set; }
    public uint CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Specialization { get; set; }
    public string? Description { get; set; }
    public string? FeesStructure { get; set; }
    public string? Languages { get; set; }
    public string FullAddress { get; set; } = string.Empty;
    public uint CityId { get; set; }
    public string? CityName { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public ProviderServiceType ServiceType { get; set; }
    public int AvgServiceDurationMins { get; set; }
    public int MaxCapacityPerDay { get; set; }
    public int AvgWaitPerTokenMins { get; set; }
    public uint PlanId { get; set; }
    public ProviderStatus Status { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Approved providers can edit these fields without re-approval. 
/// Other fields (FirmName, Email, Mobile, Domain, Category) require admin re-approval — not in this DTO.
/// </summary>
public class UpdateProviderProfileDto
{
    [MaxLength(150)]
    public string? OwnerName { get; set; }

    [RegularExpression(@"^[0-9]{10}$")]
    public string? WhatsappNumber { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(200)]
    public string? Specialization { get; set; }

    public string? Description { get; set; }
    public string? FeesStructure { get; set; }

    [MaxLength(300)]
    public string? Languages { get; set; }

    public string? FullAddress { get; set; }

    [MaxLength(10)]
    public string? Pincode { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    public ProviderServiceType? ServiceType { get; set; }

    [Range(5, 480)]
    public int? AvgServiceDurationMins { get; set; }

    [Range(1, 1000)]
    public int? MaxCapacityPerDay { get; set; }

    [Range(1, 120)]
    public int? AvgWaitPerTokenMins { get; set; }
}
