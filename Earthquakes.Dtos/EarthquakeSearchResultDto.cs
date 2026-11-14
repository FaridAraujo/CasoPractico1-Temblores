namespace Earthquakes.Dtos;

/// <summary>
/// Respuesta única del API: info del país (restcountries) + sismos (base de datos).
/// </summary>
public class EarthquakeSearchResultDto
{
    public CountryDto Country { get; set; } = new();
    public int TotalEarthquakes { get; set; }
    public List<EarthquakeDto> Earthquakes { get; set; } = new();
}
