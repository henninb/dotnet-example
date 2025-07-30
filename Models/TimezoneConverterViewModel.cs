namespace SimpleWebApp.Models;

public class TimezoneConverterViewModel
{
    public string? SourceTimezone { get; set; }
    public string? TargetTimezone { get; set; }
    public DateTime? SourceTime { get; set; }
    public DateTime? ConvertedTime { get; set; }
    public string? ConvertedTimeString { get; set; }
    public List<TimezoneInfo> AvailableTimezones { get; set; } = new();
}

public class TimezoneInfo
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}