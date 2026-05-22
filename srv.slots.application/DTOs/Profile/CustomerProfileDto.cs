using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.Profile;

public class CustomerProfileDto
{
    public uint Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}
