using Earthquakes.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Earthquakes.DataAccess.Repositories;

public interface IRepositoryBase<T>
{
    Task<bool> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<IEnumerable<T>> ReadAsync();
    Task<T?> FindAsync(int id);
}

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly EarthquakeDbContext DbContext;

    protected RepositoryBase(EarthquakeDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<bool> CreateAsync(T entity)
    {
        await DbContext.AddAsync(entity);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        DbContext.Update(entity);
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        DbContext.Remove(entity);
        return await SaveAsync();
    }

    public async Task<IEnumerable<T>> ReadAsync()
    {
        return await DbContext.Set<T>().ToListAsync();
    }

    public async Task<T?> FindAsync(int id)
    {
        return await DbContext.Set<T>().FindAsync(id);
    }

    protected async Task<bool> SaveAsync()
    {
        var result = await DbContext.SaveChangesAsync();
        return result > 0;
    }
}
