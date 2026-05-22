using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.TokenConfig;

public class TokenConfigDto
{
    public int MaxTokensPerDay { get; set; }
    public int AvgWaitPerTokenMins { get; set; }
    public TimeSpan TokenHoursStart { get; set; }
    public TimeSpan TokenHoursEnd { get; set; }
    public bool AutoResetDaily { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpsertTokenConfigDto
{
    [Range(1, 1000)]
    public int MaxTokensPerDay { get; set; } = 50;

    [Range(1, 120)]
    public int AvgWaitPerTokenMins { get; set; } = 10;

    [Required]
    public TimeSpan TokenHoursStart { get; set; }

    [Required]
    public TimeSpan TokenHoursEnd { get; set; }

    public bool AutoResetDaily { get; set; } = true;
}
