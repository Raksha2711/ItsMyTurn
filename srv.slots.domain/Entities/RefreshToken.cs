using srv.slots.domain.Enums;

namespace srv.slots.domain.Entities;

public class RefreshToken
{
    public ulong Id { get; set; }
    public UserType UserType { get; set; }
    public uint UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
