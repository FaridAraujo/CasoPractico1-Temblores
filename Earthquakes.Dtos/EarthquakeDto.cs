namespace Earthquakes.Dtos;

/// <summary>
/// Lo que sale del API hacia afuera. Nunca se expone la entidad de EF.
/// </summary>
public class EarthquakeDto
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
