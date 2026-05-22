namespace srv.slots.domain.Entities;

public class ProviderScheduleOverride
{
    public uint Id { get; set; }
    public uint ProviderId { get; set; }
    public DateTime OverrideDate { get; set; }
    public bool IsClosed { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
