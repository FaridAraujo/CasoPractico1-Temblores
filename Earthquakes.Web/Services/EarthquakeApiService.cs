using System.Text.Json;
using Earthquakes.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace Earthquakes.Web.Services;

public interface IEarthquakeApiService
{
    Task<EarthquakeSearchResultDto> SearchAsync(string country);
}

/// <summary>
/// Servicio del MVC que llama al API de temblores.
/// </summary>
public class EarthquakeApiService(HttpClient http, IMemoryCache cache) : IEarthquakeApiService
{
    public async Task<EarthquakeSearchResultDto> SearchAsync(string country)
    {
        var cacheKey = $"search_{country.ToLower()}";

        // Memory cache: la misma búsqueda no vuelve a llamar al API durante 10 minutos
        var result = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            HttpResponseMessage response;
            try
            {
                response = await http.GetAsync($"Earthquake/search?country={Uri.EscapeDataString(country)}");
            }
            catch (HttpRequestException)
            {
                throw new ApiException("No se pudo conectar con el servicio de temblores. Intenta de nuevo más tarde.", 503);
            }

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ApiException(GetErrorMessage(json), (int)response.StatusCode);

            return JsonSerializer.Deserialize<EarthquakeSearchResultDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        });

        return result!;
    }

    // El API responde { "message": "..." } cuando algo sale mal
    private static string GetErrorMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("message", out var message))
                return message.GetString() ?? "Ocurrió un error inesperado.";
        }
        catch (JsonException)
        {
        }

        return "Ocurrió un error inesperado en el servicio de temblores.";
    }
}

public class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
