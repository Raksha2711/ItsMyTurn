using srv.slots.domain.Enums;

namespace srv.slots.application.DTOs.ProviderAuth;

public class ProviderStatusDto
{
    public uint ProviderId { get; set; }
    public string FirmName { get; set; } = string.Empty;
    public ProviderStatus Status { get; set; }
    public bool IsActive { get; set; }
    public bool IsTerminated { get; set; }
    public string? RejectionReason { get; set; }
    public string? ReturnReason { get; set; }
    public string? TerminatedReason { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
}
