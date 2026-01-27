using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.Domain.Common;

/// <summary>
/// Kết quả trả về có phân trang
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedResult() { }

    public PagedResult(List<T> items, int currentPage, int pageSize, int totalCount)
    {
        Items = items;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Tạo PagedResult từ query
    /// </summary>
    public static PagedResult<T> Create(List<T> items, int currentPage, int pageSize, int totalCount)
    {
        return new PagedResult<T>(items, currentPage, pageSize, totalCount);
    }

    /// <summary>
    /// Tạo PagedResult rỗng
    /// </summary>
    public static PagedResult<T> Empty(int pageSize = 10)
    {
        return new PagedResult<T>
        {
            Items = new List<T>(),
            CurrentPage = 1,
            PageSize = pageSize,
            TotalCount = 0
        };
    }

    /// <summary>
    /// Chuyển đổi sang PaginationMeta để dùng trong ApiResponse
    /// </summary>
    public PaginationMeta ToPaginationMeta()
    {
        return new PaginationMeta(CurrentPage, PageSize, TotalCount);
    }
}