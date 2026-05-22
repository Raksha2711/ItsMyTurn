using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.ScheduleOverride;

public class ScheduleOverrideDto
{
    public uint Id { get; set; }
    public DateTime OverrideDate { get; set; }
    public bool IsClosed { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpsertScheduleOverrideDto
{
    [Required]
    public DateTime OverrideDate { get; set; }

    public bool IsClosed { get; set; }

    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }

    [MaxLength(255)]
    public string? Note { get; set; }
}
