using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHMS.Domain.Common;

/// <summary>
/// Base query parameters cho filter, sort, paging
/// </summary>
public class QueryParameters
{
    private const int MaxPageSize = 50;
    private const int DefaultPageSize = 10;
    private int _pageSize = DefaultPageSize;
    private int _pageNumber = 1;

    /// <summary>
    /// Số trang hiện tại (bắt đầu từ 1)
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Số item mỗi trang (tối đa 50)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? DefaultPageSize : value);
    }

    /// <summary>
    /// Từ khóa tìm kiếm
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Sắp xếp theo field nào
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sắp xếp giảm dần? (mặc định tăng dần)
    /// </summary>
    public bool IsDescending { get; set; } = false;

    /// <summary>
    /// Số item cần bỏ qua (dùng cho query)
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Số item cần lấy (dùng cho query)
    /// </summary>
    public int Take => PageSize;

    /// <summary>
    /// Kiểm tra có search không
    /// </summary>
    public bool HasSearch => !string.IsNullOrWhiteSpace(SearchTerm);

    /// <summary>
    /// Kiểm tra có sort không
    /// </summary>
    public bool HasSort => !string.IsNullOrWhiteSpace(SortBy);

    /// <summary>
    /// Lấy order direction string (ASC/DESC)
    /// </summary>
    public string SortDirection => IsDescending ? "DESC" : "ASC";
}

/// <summary>
/// Query parameters với filter theo ngày
/// </summary>
public class DateRangeQueryParameters : QueryParameters
{
    /// <summary>
    /// Từ ngày
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Đến ngày
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Kiểm tra có filter theo ngày không
    /// </summary>
    public bool HasDateFilter => FromDate.HasValue || ToDate.HasValue;
}

/// <summary>
/// Query parameters với filter theo trạng thái
/// </summary>
public class StatusQueryParameters : QueryParameters
{
    /// <summary>
    /// Lọc theo trạng thái
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Kiểm tra có filter theo status không
    /// </summary>
    public bool HasStatusFilter => !string.IsNullOrWhiteSpace(Status);
}

/// <summary>
/// Query parameters đầy đủ (date + status)
/// </summary>
public class FullQueryParameters : DateRangeQueryParameters
{
    /// <summary>
    /// Lọc theo trạng thái
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Kiểm tra có filter theo status không
    /// </summary>
    public bool HasStatusFilter => !string.IsNullOrWhiteSpace(Status);
}
