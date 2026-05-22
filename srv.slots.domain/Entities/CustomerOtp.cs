using srv.slots.domain.Enums;

namespace srv.slots.domain.Entities;

public class CustomerOtp
{
    public uint Id { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
}
