using srv.slots.domain.Enums;

namespace srv.slots.domain.Entities;

public class Customer
{
    public uint Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
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
