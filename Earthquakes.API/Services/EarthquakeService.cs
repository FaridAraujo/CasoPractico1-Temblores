using Earthquakes.DataAccess.Entities;
using Earthquakes.DataAccess.Repositories;
using Earthquakes.Dtos;
using Earthquakes.Dtos.Usgs;

namespace Earthquakes.API.Services;

public interface IEarthquakeService
{
    Task LoadInitialDataAsync();
    Task<int> LoadNewEarthquakesAsync();
    Task<EarthquakeSearchResultDto?> SearchByCountryAsync(string countryName);
}

public class EarthquakeService(
    IEarthquakeRepository repository,
    IUsgsService usgsService,
    IRestCountriesService countriesService,
    IConfiguration config,
    ILogger<EarthquakeService> logger) : IEarthquakeService
{
    private const int MaxResults = 100;

    /// <summary>
    /// Se ejecuta cuando arranca el API. Si la BD ya tiene datos NO llama al API de USGS.
    /// </summary>
    public async Task LoadInitialDataAsync()
    {
        if (await repository.AnyAsync())
        {
            logger.LogInformation("La base de datos ya tiene temblores, no se llama a USGS.");
            return;
        }

        var start = DateTime.Parse(config["Usgs:StartDate"]!);
        var end = DateTime.Parse(config["Usgs:EndDate"]!);

        var total = await LoadRangeAsync(start, end, null);
        logger.LogInformation("Carga inicial terminada: {Total} temblores guardados.", total);
    }

    /// <summary>
    /// Solo trae los registros nuevos (posteriores al último que tenemos guardado).
    /// </summary>
    public async Task<int> LoadNewEarthquakesAsync()
    {
        var last = await repository.GetLastTimeAsync();
        if (last == null)
        {
            await LoadInitialDataAsync();
            return 0;
        }

        var end = DateTime.UtcNow.Date.AddDays(1);
        return await LoadRangeAsync(last.Value.Date, end, last.Value);
    }

    public async Task<EarthquakeSearchResultDto?> SearchByCountryAsync(string countryName)
    {
        // 1. Info del país (API 2: REST Countries)
        var country = await countriesService.GetCountryByNameAsync(countryName);
        if (country == null)
            return null;

        // 2. Temblores de ese país (base de datos)
        var total = await repository.CountByPlaceAsync(country.Name);
        var earthquakes = await repository.SearchByPlaceAsync(country.Name, MaxResults);

        // 3. Un único objeto con todo
        return new EarthquakeSearchResultDto
        {
            Country = country,
            TotalEarthquakes = total,
            Earthquakes = earthquakes.Select(ConvertToDto).ToList()
        };
    }

    // USGS solo permite 20 000 resultados por consulta, por eso se pide mes por mes.
    private async Task<int> LoadRangeAsync(DateTime start, DateTime end, DateTime? onlyAfter)
    {
        var total = 0;

        for (var from = start; from < end; from = from.AddMonths(1))
        {
            var to = from.AddMonths(1) < end ? from.AddMonths(1) : end;

            var response = await usgsService.GetEarthquakesAsync(from, to);
            var entities = response.Features
                .Select(ConvertToEntity)
                .Where(e => onlyAfter == null || e.Time > onlyAfter)
                .ToList();

            if (entities.Count > 0)
            {
                await repository.CreateManyAsync(entities);
                total += entities.Count;
            }
        }

        return total;
    }

    private static Earthquake ConvertToEntity(UsgsFeature f)
    {
        var coordinates = f.Geometry.Coordinates;
        return new Earthquake
        {
            UsgsId = f.Id,
            Magnitude = f.Properties.Mag,
            Place = f.Properties.Place,
            Time = DateTimeOffset.FromUnixTimeMilliseconds(f.Properties.Time).UtcDateTime,
            Url = f.Properties.Url,
            Longitude = coordinates.Count > 0 ? coordinates[0] : null,
            Latitude = coordinates.Count > 1 ? coordinates[1] : null,
            Depth = coordinates.Count > 2 ? coordinates[2] : null
        };
    }

    private static EarthquakeDto ConvertToDto(Earthquake e)
    {
        return new EarthquakeDto
        {
            Id = e.Id,
            UsgsId = e.UsgsId,
            Magnitude = e.Magnitude,
            Place = e.Place,
            Time = e.Time,
            Latitude = e.Latitude,
            Longitude = e.Longitude,
            Depth = e.Depth,
            Url = e.Url
        };
    }
}
