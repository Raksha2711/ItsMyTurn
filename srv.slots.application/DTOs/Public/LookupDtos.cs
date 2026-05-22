namespace srv.slots.application.DTOs.Public;

public class CountryDto
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class StateDto
{
    public uint Id { get; set; }
    public uint CountryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}

public class CityDto
{
    public uint Id { get; set; }
    public uint StateId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class BoundaryDto
{
    public uint Id { get; set; }
    public uint CityId { get; set; }
    public string? AreaName { get; set; }
    public string? Pincode { get; set; }
    public decimal? LatitudeCenter { get; set; }
    public decimal? LongitudeCenter { get; set; }
    public decimal? RadiusKm { get; set; }
}
