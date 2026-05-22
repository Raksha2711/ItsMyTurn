namespace srv.slots.domain.Entities;

public class ProviderWorkingHour
{
    public uint Id { get; set; }
    public uint ProviderId { get; set; }
    public byte DayOfWeek { get; set; }    // 0=Sun, 6=Sat
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsDayOff { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
