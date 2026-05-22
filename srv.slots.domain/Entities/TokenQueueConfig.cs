namespace srv.slots.domain.Entities;

public class TokenQueueConfig
{
    public uint Id { get; set; }
    public uint ProviderId { get; set; }
    public int MaxTokensPerDay { get; set; } = 50;
    public int AvgWaitPerTokenMins { get; set; } = 10;
    public TimeSpan TokenHoursStart { get; set; } = new(9, 0, 0);
    public TimeSpan TokenHoursEnd { get; set; } = new(18, 0, 0);
    public bool AutoResetDaily { get; set; } = true;
    public DateTime UpdatedAt { get; set; }
}
