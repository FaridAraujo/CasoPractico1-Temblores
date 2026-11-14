using System.Text.Json;
using Earthquakes.Dtos.Usgs;
using Microsoft.Extensions.Caching.Memory;

namespace Earthquakes.API.Services;

public interface IUsgsService
{
    Task<UsgsResponse> GetEarthquakesAsync(DateTime start, DateTime end);
}

/// <summary>
/// Servicio que llama al API de USGS (temblores/terremotos).
/// </summary>
public class UsgsService(HttpClient http, IMemoryCache cache, IConfiguration config) : IUsgsService
{
    public async Task<UsgsResponse> GetEarthquakesAsync(DateTime start, DateTime end)
    {
        var minMagnitude = config["Usgs:MinMagnitude"];
        var url = $"query?format=geojson&starttime={start:yyyy-MM-dd}&endtime={end:yyyy-MM-dd}&minmagnitude={minMagnitude}";

        // Memory cache: si ya pedimos exactamente este rango hace poco, no volvemos a llamar al API
        var result = await cache.GetOrCreateAsync(url, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UsgsResponse>(json) ?? new UsgsResponse();
        });

        return result!;
    }
}
