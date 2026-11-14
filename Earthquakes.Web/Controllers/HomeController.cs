using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Earthquakes.Web.Models;
using Earthquakes.Web.Services;

namespace Earthquakes.Web.Controllers;

public class HomeController : Controller
{
    private readonly IEarthquakeApiService _earthquakeApiService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IEarthquakeApiService earthquakeApiService, ILogger<HomeController> logger)
    {
        _earthquakeApiService = earthquakeApiService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new SearchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SearchViewModel model)
    {
        try
        {
            // Input validation: requerido, solo letras, máximo 100 caracteres
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .First().ErrorMessage;
                model.ErrorIcon = "warning";
                return View(model);
            }

            model.Result = await _earthquakeApiService.SearchAsync(model.Country!.Trim());
        }
        catch (ApiException ex)
        {
            // El API respondió con un error: lo mostramos amigable
            _logger.LogWarning(ex, "El API respondió con error {StatusCode}", ex.StatusCode);
            model.ErrorMessage = ex.Message;
            model.ErrorIcon = ex.StatusCode < 500 ? "warning" : "error";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en la búsqueda");
            model.ErrorMessage = "Ocurrió un problema al hacer la búsqueda. Intenta de nuevo más tarde.";
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
