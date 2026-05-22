namespace srv.slots.domain.Entities;

public class ProviderSlot
{
    public uint Id { get; set; }
    public uint ProviderId { get; set; }
    public uint? ServiceId { get; set; }
    public DateTime SlotDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int MaxBookings { get; set; } = 1;
    public int BookedCount { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
