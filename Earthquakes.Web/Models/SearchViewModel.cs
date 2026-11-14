using System.ComponentModel.DataAnnotations;
using Earthquakes.Dtos;

namespace Earthquakes.Web.Models;

public class SearchViewModel
{
    // Solo letras (con tildes), espacios y guion. Sin números ni caracteres especiales.
    [Required(ErrorMessage = "Escribe el nombre de un país.")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    [RegularExpression(@"^\s*[\p{L}]+([ \-][\p{L}]+)*\s*$",
        ErrorMessage = "Escribe solo el nombre de un país, sin números ni caracteres especiales.")]
    public string? Country { get; set; }

    public EarthquakeSearchResultDto? Result { get; set; }

    // Si hay error, la vista lo muestra con SweetAlert2
    public string? ErrorMessage { get; set; }
    public string ErrorIcon { get; set; } = "error";
}
