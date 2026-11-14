using System.Text.Json.Serialization;

namespace Earthquakes.Dtos.Usgs;

// DTOs para deserializar la respuesta GeoJSON de USGS

public class UsgsResponse
{
    [JsonPropertyName("features")]
    public List<UsgsFeature> Features { get; set; } = new();
}

public class UsgsFeature
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("properties")]
    public UsgsProperties Properties { get; set; } = new();

    [JsonPropertyName("geometry")]
    public UsgsGeometry Geometry { get; set; } = new();
}

public class UsgsProperties
{
    [JsonPropertyName("mag")]
    public decimal? Mag { get; set; }

    [JsonPropertyName("place")]
    public string? Place { get; set; }

    // Milisegundos desde 1970 (epoch)
    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class UsgsGeometry
{
    // [longitud, latitud, profundidad]
    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; set; } = new();
}
