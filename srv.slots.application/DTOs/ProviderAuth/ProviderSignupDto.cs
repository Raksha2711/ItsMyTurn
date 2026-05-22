using System.ComponentModel.DataAnnotations;
using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.ProviderAuth;

public class ProviderSignupDto
{
    // Basic Info
    [Required, MaxLength(200)]
    public string FirmName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string OwnerName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(191)]
    public string Email { get; set; } = string.Empty;

    [Required, RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile must be 10 digits")]
    public string Mobile { get; set; } = string.Empty;

    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "WhatsApp must be 10 digits")]
    public string? WhatsappNumber { get; set; }

    [Required, MinLength(6), MaxLength(50)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    // Category
    [Required]
    public uint DomainId { get; set; }

    [Required]
    public uint CategoryId { get; set; }

    [MaxLength(200)]
    public string? Specialization { get; set; }

    public string? Description { get; set; }
    public string? FeesStructure { get; set; }

    [MaxLength(300)]
    public string? Languages { get; set; }

    // Location
    [Required]
    public string FullAddress { get; set; } = string.Empty;

    [Required]
    public uint CityId { get; set; }

    [MaxLength(10)]
    public string? Pincode { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    public uint? BoundaryId { get; set; }

    // Service config
    public ProviderServiceType ServiceType { get; set; } = ProviderServiceType.Appointment;

    [Range(5, 480)]
    public int AvgServiceDurationMins { get; set; } = 15;

    [Range(1, 1000)]
    public int MaxCapacityPerDay { get; set; } = 50;

    [Range(1, 120)]
    public int AvgWaitPerTokenMins { get; set; } = 10;
}
