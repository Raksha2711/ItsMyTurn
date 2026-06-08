namespace srv.slots.domain.Entities;

public class Appointment
{
    public uint     Id                { get; set; }
    public string   BookingRef        { get; set; } = "";          // #BK00001
    public string   AppointmentType   { get; set; } = "Slot";      // Slot | Token
    public uint     CustomerId        { get; set; }
    public uint     ProviderId        { get; set; }
    public uint?    ServiceId         { get; set; }
    public uint?    SlotId            { get; set; }
    public DateTime AppointmentDate   { get; set; }
    public TimeSpan? AppointmentTime  { get; set; }
    public int?     TokenNumber       { get; set; }
    public uint?    TokenQueueId      { get; set; }
    public string?  PatientName       { get; set; }
    public int?     PatientAge        { get; set; }
    public string?  VisitReason       { get; set; }
    public string   Status            { get; set; } = "Pending";
    public string?  RejectionReason   { get; set; }
    public string?  CancellationReason { get; set; }
    public string?  CancelledBy       { get; set; }
    public string?  ProviderNote      { get; set; }
    public DateTime CreatedAt         { get; set; }
    public DateTime UpdatedAt         { get; set; }
}
