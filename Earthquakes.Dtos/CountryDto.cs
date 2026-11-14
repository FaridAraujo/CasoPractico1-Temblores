namespace Earthquakes.Dtos;

public class CountryDto
{
    public string Name { get; set; } = string.Empty;
    public string OfficialName { get; set; } = string.Empty;
    public string? Capital { get; set; }
    public string? Region { get; set; }
    public string? Subregion { get; set; }
    public long Population { get; set; }
    public string? FlagUrl { get; set; }
    public string? Currencies { get; set; }
    public string? Languages { get; set; }
}
