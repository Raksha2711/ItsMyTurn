using System.ComponentModel.DataAnnotations;

namespace srv.slots.application.DTOs.Appointment;

// ── Provider view ──────────────────────────────────────────────────────────────
public class AppointmentDto
{
    public uint      Id                 { get; set; }
    public string    BookingRef         { get; set; } = "";
    public string    AppointmentType    { get; set; } = "Slot";
    public DateTime  AppointmentDate    { get; set; }
    public TimeSpan? AppointmentTime    { get; set; }
    public int?      TokenNumber        { get; set; }
    public string    Status             { get; set; } = "";
    public string?   PatientName        { get; set; }
    public int?      PatientAge         { get; set; }
    public string?   VisitReason        { get; set; }
    public string?   ProviderNote       { get; set; }
    public string?   RejectionReason    { get; set; }
    public string?   CancellationReason { get; set; }
    public string?   CancelledBy        { get; set; }
    public DateTime  CreatedAt          { get; set; }

    // Customer
    public uint   CustomerId     { get; set; }
    public string CustomerName   { get; set; } = "";
    public string CustomerMobile { get; set; } = "";

    // Service + slot times
    public uint?   ServiceId   { get; set; }
    public string? ServiceName { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime   { get; set; }
}

// ── Customer view ──────────────────────────────────────────────────────────────
public class CustomerAppointmentDto
{
    public uint      Id              { get; set; }
    public string    BookingRef      { get; set; } = "";
    public DateTime  AppointmentDate { get; set; }
    public TimeSpan? AppointmentTime { get; set; }
    public int?      TokenNumber     { get; set; }
    public string    Status          { get; set; } = "";
    public string?   PatientName     { get; set; }
    public string?   VisitReason     { get; set; }
    public DateTime  CreatedAt       { get; set; }

    // Provider
    public uint   ProviderId { get; set; }
    public string FirmName   { get; set; } = "";

    // Service
    public string? ServiceName { get; set; }
}

// ── Booking request (customer) ─────────────────────────────────────────────────
public class BookAppointmentDto
{
    [Required] public uint    SlotId      { get; set; }
    public             string? PatientName { get; set; }
    public             int?    PatientAge  { get; set; }
    public             string? VisitReason { get; set; }
}

// ── Status update request (provider) ─────────────────────────────────────────
public class UpdateAppointmentStatusDto
{
    /// <summary>Confirmed | Waiting | Ongoing | Completed | Skipped | Cancelled | Rejected | NoShow</summary>
    [Required] public string  Status             { get; set; } = "";
    public             string? RejectionReason    { get; set; }
    public             string? CancellationReason { get; set; }
    public             string? ProviderNote       { get; set; }
}

// ── Dashboard stats ────────────────────────────────────────────────────────────
public class AppointmentStatsDto
{
    public int TotalToday { get; set; }
    public int Pending    { get; set; }
    public int Confirmed  { get; set; }
    public int Waiting    { get; set; }
    public int Ongoing    { get; set; }
    public int Completed  { get; set; }
    public int Skipped    { get; set; }
    public int Cancelled  { get; set; }
    public int Rejected   { get; set; }
    public int NoShows    { get; set; }
}
