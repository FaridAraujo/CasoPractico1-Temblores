using System.Net.Http.Headers;
using System.Text.Json;
using Earthquakes.Dtos;
using Earthquakes.Dtos.Countries;
using Microsoft.Extensions.Caching.Memory;

namespace Earthquakes.API.Services;

public interface IRestCountriesService
{
    Task<CountryDto?> GetCountryByNameAsync(string name);
}

/// <summary>
/// Servicio que llama al API de REST Countries (info del país). Requiere API key.
/// </summary>
public class RestCountriesService(HttpClient http, IMemoryCache cache, IConfiguration config) : IRestCountriesService
{
    public async Task<CountryDto?> GetCountryByNameAsync(string name)
    {
        var apiKey = config["RestCountries:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.StartsWith("PEGA_AQUI"))
            throw new InvalidOperationException("Falta configurar la API key de REST Countries en el appsettings.json del API.");

        var cacheKey = $"country_{name.ToLower()}";

        // Memory cache: el mismo país no se vuelve a consultar durante 1 hora
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            // 1. Busca por nombre (inglés). 2. Si no hay resultados, busca en las traducciones (ej. "Canadá", "Alemania")
            var countries = await SearchAsync("name", name, apiKey);
            if (countries.Count == 0)
                countries = await SearchAsync("names.translations", name, apiKey);

            // Si hay varios resultados, preferimos el que se llama exactamente igual
            var country = countries.FirstOrDefault(c => c.Names.Common.Equals(name, StringComparison.OrdinalIgnoreCase))
                          ?? countries.FirstOrDefault();

            return country == null ? null : ConvertToDto(country);
        });
    }

    private async Task<List<RestCountry>> SearchAsync(string field, string name, string apiKey)
    {
        var url = $"countries/v5/{field}?q={Uri.EscapeDataString(name)}&limit=5";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await http.SendAsync(request);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return new List<RestCountry>();
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<RestCountriesResponse>(json);
        return data?.Data.Objects ?? new List<RestCountry>();
    }

    private static CountryDto ConvertToDto(RestCountry c)
    {
        return new CountryDto
        {
            Name = c.Names.Common,
            OfficialName = c.Names.Official,
            Capital = c.Capitals.FirstOrDefault()?.Name,
            Region = c.Region,
            Subregion = c.Subregion,
            Population = c.Population,
            FlagUrl = c.Flag.UrlPng,
            Currencies = string.Join(", ", c.Currencies.Select(x => $"{x.Name} ({x.Symbol})")),
            Languages = string.Join(", ", c.Languages.Select(x => x.Name))
        };
    }
}
