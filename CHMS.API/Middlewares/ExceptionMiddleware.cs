using System.Net;
using System.Text.Json;
using CHMS.BLL.Common.Exceptions;
using CHMS.Domain.Common;

namespace CHMS.API.Middlewares;

/// <summary>
/// Middleware xử lý exception toàn cục
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse<object>();

        switch (exception)
        {
            case NotFoundException notFound:
                context.Response.StatusCode = 404;
                response = ApiResponse<object>.ErrorResult(notFound.Message);
                break;

            case BadRequestException badRequest:
                context.Response.StatusCode = 400;
                response = ApiResponse<object>.ErrorResult(badRequest.Message);
                break;

            case UnauthorizedException unauthorized:
                context.Response.StatusCode = 401;
                response = ApiResponse<object>.ErrorResult(unauthorized.Message);
                break;

            // ... các case khác

            default:
                context.Response.StatusCode = 500;
                response = ApiResponse<object>.ErrorResult("Đã xảy ra lỗi.");
                break;
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension method để đăng ký middleware
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}