namespace srv.slots.domain.Entities;

public class CustomerAddress
{
    public uint Id { get; set; }
    public uint CustomerId { get; set; }
    public string Label { get; set; } = string.Empty;   // Home, Work, Other
    public string FullAddress { get; set; } = string.Empty;
    public uint CityId { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
