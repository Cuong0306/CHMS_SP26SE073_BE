using System.Linq.Expressions;
using CHMS.Domain.Common;

namespace CHMS.DAL.Common;

/// <summary>
/// Interface base cho tất cả repositories
/// </summary>
public interface IGenericRepository<T> where T : class
{
    #region Get Methods

    /// <summary>
    /// Lấy entity theo Id
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Lấy entity theo điều kiện
    /// </summary>
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Lấy entity theo điều kiện với includes
    /// </summary>
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Lấy tất cả entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Lấy tất cả entities theo điều kiện
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Lấy tất cả entities với includes
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Lấy tất cả entities theo điều kiện với includes
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    #endregion

    #region Paging Methods

    /// <summary>
    /// Lấy danh sách có phân trang
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(QueryParameters parameters);

    /// <summary>
    /// Lấy danh sách có phân trang theo điều kiện
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(QueryParameters parameters, Expression<Func<T, bool>> predicate);

    #endregion

    #region Check Methods

    /// <summary>
    /// Kiểm tra có tồn tại theo điều kiện không
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Đếm số lượng theo điều kiện
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    #endregion

    #region CUD Methods

    /// <summary>
    /// Thêm entity mới
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Thêm nhiều entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Cập nhật entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Cập nhật nhiều entities
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Xóa entity (hard delete)
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Xóa nhiều entities (hard delete)
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Xóa mềm entity
    /// </summary>
    Task SoftDeleteAsync(int id);

    #endregion

    #region Query Methods

    /// <summary>
    /// Trả về IQueryable để custom query
    /// </summary>
    IQueryable<T> Query();

    /// <summary>
    /// Trả về IQueryable với điều kiện
    /// </summary>
    IQueryable<T> Query(Expression<Func<T, bool>> predicate);

    #endregion
}