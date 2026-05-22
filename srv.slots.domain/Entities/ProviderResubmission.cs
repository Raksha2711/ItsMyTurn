namespace srv.slots.domain.Entities;

public class ProviderResubmission
{
    public uint Id { get; set; }
    public uint ProviderId { get; set; }
    public string ReturnReason { get; set; } = string.Empty;
    public uint ReturnedBy { get; set; }
    public DateTime ReturnedAt { get; set; }
    public DateTime? ResubmittedAt { get; set; }
}
