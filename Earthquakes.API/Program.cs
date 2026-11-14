using Earthquakes.API.Services;
using Earthquakes.DataAccess.Data;
using Earthquakes.DataAccess.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<EarthquakeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EarthquakeDb"),
        sql => sql.CommandTimeout(180))); // la búsqueda por texto recorre ~140 000 filas
builder.Services.AddScoped<IEarthquakeRepository, EarthquakeRepository>();

builder.Services.AddScoped<IEarthquakeService, EarthquakeService>();
builder.Services.AddHttpClient<IUsgsService, UsgsService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Usgs:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(60);
});
builder.Services.AddHttpClient<IRestCountriesService, RestCountriesService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["RestCountries:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Cualquier error no controlado responde un JSON con mensaje (nunca una página de error)
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    app.Logger.LogError(error, "Error no controlado en el API");

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { message = "Ocurrió un error en el servidor." });
}));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Primera carga: si la BD está vacía se llena con los datos de USGS
using (var scope = app.Services.CreateScope())
{
    try
    {
        var earthquakeService = scope.ServiceProvider.GetRequiredService<IEarthquakeService>();
        await earthquakeService.LoadInitialDataAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "No se pudo hacer la carga inicial de temblores.");
    }
}

app.Run();
