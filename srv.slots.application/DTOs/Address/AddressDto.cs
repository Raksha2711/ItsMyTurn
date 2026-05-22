namespace srv.slots.application.DTOs.Address;

public class AddressDto
{
    public uint Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public uint CityId { get; set; }
    public string? CityName { get; set; }
    public string? StateName { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}
