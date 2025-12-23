using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly POSDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(POSDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        _dbSet.Update(entity);
        return await Task.FromResult(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        return true;
    }
}
