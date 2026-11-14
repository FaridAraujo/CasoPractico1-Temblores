using Earthquakes.DataAccess.Data;
using Earthquakes.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Earthquakes.DataAccess.Repositories;

public interface IEarthquakeRepository : IRepositoryBase<Earthquake>
{
    Task<bool> AnyAsync();
    Task<DateTime?> GetLastTimeAsync();
    Task<bool> CreateManyAsync(IEnumerable<Earthquake> earthquakes);
    Task<int> CountByPlaceAsync(string place);
    Task<List<Earthquake>> SearchByPlaceAsync(string place, int take);
}

public class EarthquakeRepository : RepositoryBase<Earthquake>, IEarthquakeRepository
{
    public EarthquakeRepository(EarthquakeDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> AnyAsync()
    {
        return await DbContext.Earthquakes.AnyAsync();
    }

    public async Task<DateTime?> GetLastTimeAsync()
    {
        return await DbContext.Earthquakes.MaxAsync(e => (DateTime?)e.Time);
    }

    public async Task<bool> CreateManyAsync(IEnumerable<Earthquake> earthquakes)
    {
        await DbContext.Earthquakes.AddRangeAsync(earthquakes);
        var saved = await SaveAsync();
        DbContext.ChangeTracker.Clear(); // libera memoria entre cargas
        return saved;
    }

    public async Task<int> CountByPlaceAsync(string place)
    {
        return await DbContext.Earthquakes.CountAsync(e => e.Place!.Contains(place));
    }

    public async Task<List<Earthquake>> SearchByPlaceAsync(string place, int take)
    {
        return await DbContext.Earthquakes
            .AsNoTracking()
            .Where(e => e.Place!.Contains(place))
            .OrderByDescending(e => e.Time)
            .Take(take)
            .ToListAsync();
    }
}
