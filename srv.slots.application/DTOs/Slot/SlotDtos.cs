using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Slot;

public class SlotDto
{
    public uint Id { get; set; }
    public uint? ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public DateTime SlotDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int MaxBookings { get; set; }
    public int BookedCount { get; set; }
    public int RemainingCapacity => MaxBookings - BookedCount;
    public bool IsBlocked { get; set; }
    public bool IsActive { get; set; }
}

public class GenerateSlotsDto
{
    /// <summary>Start date (inclusive)</summary>
    [Required]
    public DateTime FromDate { get; set; }

    /// <summary>End date (inclusive). Max 90 days from FromDate.</summary>
    [Required]
    public DateTime ToDate { get; set; }

    /// <summary>If null, slot is generic (not tied to a service).</summary>
    public uint? ServiceId { get; set; }

    /// <summary>Duration per slot in minutes (e.g. 15, 30, 60).</summary>
    [Range(5, 480)]
    public int SlotDurationMins { get; set; } = 30;

    /// <summary>How many people can book each slot.</summary>
    [Range(1, 100)]
    public int MaxBookingsPerSlot { get; set; } = 1;

    /// <summary>If true, skips dates that already have slots. If false, throws if conflicts.</summary>
    public bool SkipExisting { get; set; } = true;
}

public class GenerateSlotsResultDto
{
    public int TotalSlotsCreated { get; set; }
    public int DatesProcessed { get; set; }
    public int DatesSkipped { get; set; }
    public List<string> Notes { get; set; } = new();
}
