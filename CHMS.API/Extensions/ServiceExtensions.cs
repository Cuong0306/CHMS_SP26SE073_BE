using CHMS.DAL.Common;
using CHMS.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CHMS.API.Extensions;

/// <summary>
/// Extension methods để đăng ký services cho Dependency Injection
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Đăng ký DbContext
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CoastalHomestayDBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        return services;
    }

    /// <summary>
    /// Đăng ký tất cả Repositories
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Specific Repositories - Thêm khi tạo
        // services.AddScoped<IUserRepository, UserRepository>();
        // services.AddScoped<IHomestayRepository, HomestayRepository>();
        // services.AddScoped<IBookingRepository, BookingRepository>();
        // services.AddScoped<IReviewRepository, ReviewRepository>();
        // services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }

    /// <summary>
    /// Đăng ký tất cả Services (BLL)
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Services - Thêm khi tạo
        // services.AddScoped<IAuthService, AuthService>();
        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<IHomestayService, HomestayService>();
        // services.AddScoped<IBookingService, BookingService>();
        // services.AddScoped<IReviewService, ReviewService>();
        // services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }

    /// <summary>
    /// Đăng ký Third Party Services
    /// </summary>
    public static IServiceCollection AddThirdPartyServices(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Service
        // services.AddScoped<IJwtService, JwtService>();

        // Email Service
        // services.AddScoped<IEmailService, SendGridService>();

        // Storage Service (Cloudinary)
        // services.AddScoped<IStorageService, CloudinaryService>();

        // Payment Service (VNPay)
        // services.AddScoped<IPaymentGatewayService, VNPayService>();

        return services;
    }

    /// <summary>
    /// Đăng ký AutoMapper
    /// </summary>
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        // Scan và đăng ký tất cả profiles trong assembly của BLL
        // services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

        return services;
    }

    /// <summary>
    /// Cấu hình CORS
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            // Production policy - chỉ cho phép specific origins
            options.AddPolicy("Production", policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Cấu hình Swagger
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Coastal Homestay API",
                Version = "v1",
                Description = "API cho hệ thống quản lý Homestay ven biển"
            });

            // Thêm JWT authentication vào Swagger
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Nhập JWT token theo format: Bearer {token}"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Cấu hình SignalR
    /// </summary>
    public static IServiceCollection AddSignalRHubs(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });

        return services;
    }
}