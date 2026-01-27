using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace CHMS.Domain.Common;

/// <summary>
/// Response wrapper chuẩn cho tất cả API
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationMeta? Pagination { get; set; }

    #region Factory Methods - Success

    /// <summary>
    /// Trả về response thành công với data
    /// </summary>
    public static ApiResponse<T> SuccessResult(T data, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Trả về response thành công không có data
    /// </summary>
    public static ApiResponse<T> SuccessResult(string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// Trả về response thành công với data và phân trang
    /// </summary>
    public static ApiResponse<T> SuccessResult(T data, PaginationMeta pagination, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = pagination
        };
    }

    #endregion

    #region Factory Methods - Error

    /// <summary>
    /// Trả về response lỗi
    /// </summary>
    public static ApiResponse<T> ErrorResult(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }

    /// <summary>
    /// Trả về response lỗi với danh sách lỗi chi tiết
    /// </summary>
    public static ApiResponse<T> ErrorResult(string message, List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    /// <summary>
    /// Trả về response validation error
    /// </summary>
    public static ApiResponse<T> ValidationError(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Dữ liệu không hợp lệ",
            Errors = errors
        };
    }

    #endregion
}

/// <summary>
/// Response không có data (dùng cho Delete, Update không cần return data)
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public new static ApiResponse SuccessResult(string message = "Thành công")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public new static ApiResponse ErrorResult(string message)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message
        };
    }

    public new static ApiResponse ErrorResult(string message, List<string> errors)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// Thông tin phân trang
/// </summary>
public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }

    public PaginationMeta() { }

    public PaginationMeta(int currentPage, int pageSize, int totalCount)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        HasPrevious = currentPage > 1;
        HasNext = currentPage < TotalPages;
    }
}
