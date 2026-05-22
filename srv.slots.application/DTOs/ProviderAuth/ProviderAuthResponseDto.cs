using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.ProviderAuth;

public class ProviderAuthResponseDto
{
    public uint ProviderId { get; set; }
    public string FirmName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public ProviderStatus Status { get; set; }
    public string? ReturnReason { get; set; }
    public string? RejectionReason { get; set; }

    /// <summary>Will be null if status != Approved (no login token issued).</summary>
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }

    public string Message { get; set; } = string.Empty;
}
