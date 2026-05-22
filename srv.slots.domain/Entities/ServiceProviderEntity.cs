using srv.slots.domain.Enums;

namespace srv.slots.domain.Entities;

// Note: named with full namespace usage in code to avoid clash with System.IServiceProvider
public class ServiceProviderEntity
{
    public uint Id { get; set; }
    public string FirmName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? WhatsappNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }

    public uint DomainId { get; set; }
    public uint CategoryId { get; set; }
    public string? Specialization { get; set; }
    public string? Description { get; set; }
    public string? FeesStructure { get; set; }
    public string? Languages { get; set; }

    public string FullAddress { get; set; } = string.Empty;
    public uint CityId { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public uint? BoundaryId { get; set; }

    public ProviderServiceType ServiceType { get; set; } = ProviderServiceType.Appointment;
    public int AvgServiceDurationMins { get; set; } = 15;
    public int MaxCapacityPerDay { get; set; } = 50;
    public int AvgWaitPerTokenMins { get; set; } = 10;

    public uint PlanId { get; set; } = 1;
    public DateTime? PlanStartDate { get; set; }
    public DateTime? PlanEndDate { get; set; }

    public ProviderStatus Status { get; set; } = ProviderStatus.Pending;
    public string? RejectionReason { get; set; }
    public string? ReturnReason { get; set; }
    public uint? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public bool IsVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTerminated { get; set; }
    public uint? TerminatedBy { get; set; }
    public DateTime? TerminatedAt { get; set; }
    public string? TerminatedReason { get; set; }

    public string? FcmToken { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
