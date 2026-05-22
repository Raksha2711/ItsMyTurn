namespace srv.slots.application.DTOs.Provider;

public class ProviderDetailDto
{
    public uint Id { get; set; }
    public string FirmName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? FeesStructure { get; set; }
    public string? Languages { get; set; }

    public uint DomainId { get; set; }
    public string? DomainName { get; set; }
    public uint CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public string FullAddress { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public string? Pincode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public string ServiceType { get; set; } = string.Empty;
    public int AvgServiceDurationMins { get; set; }
    public int AvgWaitPerTokenMins { get; set; }

    public string? Mobile { get; set; }
    public string? WhatsappNumber { get; set; }

    public decimal AvgRating { get; set; }
    public int ReviewCount { get; set; }

    public List<ProviderServiceDto> Services { get; set; } = new();
    public List<WorkingHourDto> WorkingHours { get; set; } = new();
}

public class ProviderServiceDto
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMins { get; set; }
    public decimal Price { get; set; }
    public string? PriceLabel { get; set; }
    public uint? SubCategoryId { get; set; }
    public string? SubCategoryName { get; set; }
}

public class WorkingHourDto
{
    public byte DayOfWeek { get; set; }   // 0=Sun..6=Sat
    public string? DayName { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsDayOff { get; set; }
}
