namespace Earthquakes.DataAccess.Entities;

public class Earthquake
{
    public int Id { get; set; }
    public string UsgsId { get; set; } = string.Empty;
    public decimal? Magnitude { get; set; }
    public string? Place { get; set; }
    public DateTime Time { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Depth { get; set; }
    public string? Url { get; set; }
}
