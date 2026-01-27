using System.Linq.Expressions;
using CHMS.DAL.Data;
using CHMS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CHMS.DAL.Common;

/// <summary>
/// Implementation base cho tất cả repositories
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly CoastalHomestayDBContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(CoastalHomestayDBContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    #region Get Methods

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    #endregion

    #region Paging Methods

    public virtual async Task<PagedResult<T>> GetPagedAsync(QueryParameters parameters)
    {
        var query = _dbSet.AsQueryable();
        return await ApplyPagingAsync(query, parameters);
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(QueryParameters parameters, Expression<Func<T, bool>> predicate)
    {
        var query = _dbSet.Where(predicate);
        return await ApplyPagingAsync(query, parameters);
    }

    protected async Task<PagedResult<T>> ApplyPagingAsync(IQueryable<T> query, QueryParameters parameters)
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToListAsync();

        return PagedResult<T>.Create(items, parameters.PageNumber, parameters.PageSize, totalCount);
    }

    #endregion

    #region Check Methods

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();

        return await _dbSet.CountAsync(predicate);
    }

    #endregion

    #region CUD Methods

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual void Delete(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task SoftDeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            // Dùng reflection để set IsDeleted và DeletedAt
            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
            var deletedAtProperty = typeof(T).GetProperty("DeletedAt");

            if (isDeletedProperty != null)
            {
                isDeletedProperty.SetValue(entity, true);
            }

            if (deletedAtProperty != null)
            {
                deletedAtProperty.SetValue(entity, DateTime.Now);
            }

            Update(entity);
        }
    }

    #endregion

    #region Query Methods

    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    #endregion
}