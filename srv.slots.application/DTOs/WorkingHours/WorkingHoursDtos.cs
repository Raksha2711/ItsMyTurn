using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.WorkingHours;

public class WorkingHourDayDto
{
    [Range(0, 6)]
    public byte DayOfWeek { get; set; }   // 0=Sun, 6=Sat

    public string? DayName { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsDayOff { get; set; }
}

public class UpsertWeekDto
{
    [Required, MinLength(1), MaxLength(7)]
    public List<WorkingHourDayDto> Days { get; set; } = new();
}
