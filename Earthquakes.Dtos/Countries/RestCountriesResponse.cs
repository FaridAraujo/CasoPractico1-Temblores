using System.Text.Json.Serialization;

namespace Earthquakes.Dtos.Countries;

// DTOs para deserializar la respuesta de REST Countries (v5)

public class RestCountriesResponse
{
    [JsonPropertyName("data")]
    public RestCountriesData Data { get; set; } = new();
}

public class RestCountriesData
{
    [JsonPropertyName("objects")]
    public List<RestCountry> Objects { get; set; } = new();
}

public class RestCountry
{
    [JsonPropertyName("names")]
    public RestCountryNames Names { get; set; } = new();

    [JsonPropertyName("capitals")]
    public List<RestCountryCapital> Capitals { get; set; } = new();

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("subregion")]
    public string? Subregion { get; set; }

    [JsonPropertyName("population")]
    public long Population { get; set; }

    [JsonPropertyName("flag")]
    public RestCountryFlag Flag { get; set; } = new();

    [JsonPropertyName("currencies")]
    public List<RestCountryCurrency> Currencies { get; set; } = new();

    [JsonPropertyName("languages")]
    public List<RestCountryLanguage> Languages { get; set; } = new();
}

public class RestCountryNames
{
    [JsonPropertyName("common")]
    public string Common { get; set; } = string.Empty;

    [JsonPropertyName("official")]
    public string Official { get; set; } = string.Empty;
}

public class RestCountryCapital
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class RestCountryFlag
{
    [JsonPropertyName("url_png")]
    public string? UrlPng { get; set; }
}

public class RestCountryCurrency
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}

public class RestCountryLanguage
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
