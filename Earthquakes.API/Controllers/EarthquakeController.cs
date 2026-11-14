using Earthquakes.API.Services;
using Earthquakes.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Earthquakes.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EarthquakeController(IEarthquakeService earthquakeService) : ControllerBase
{
    /// <summary>
    /// Busca temblores por nombre de país y trae la info del país en el mismo objeto.
    /// </summary>
    [HttpGet("search", Name = "SearchByCountry")]
    public async Task<ActionResult<EarthquakeSearchResultDto>> Search([FromQuery] string country)
    {
        if (string.IsNullOrWhiteSpace(country) || country.Length > 100)
            return BadRequest(new { message = "Debe indicar un nombre de país de máximo 100 caracteres." });

        var result = await earthquakeService.SearchByCountryAsync(country.Trim());
        if (result == null)
            return NotFound(new { message = $"No se encontró el país '{country}'." });

        return result;
    }

    /// <summary>
    /// Solicita únicamente los registros nuevos a USGS (los posteriores al último guardado).
    /// </summary>
    [HttpPost("refresh", Name = "RefreshEarthquakes")]
    public async Task<ActionResult<int>> Refresh()
    {
        return await earthquakeService.LoadNewEarthquakesAsync();
    }
}
