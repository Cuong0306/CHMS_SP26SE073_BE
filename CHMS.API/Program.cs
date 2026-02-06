using CHMS.API.Extensions;
using CHMS.API.Middlewares;
using CHMS.BLL.Services.Interfaces;
using CHMS.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using CHMS.BLL.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//================================
// Add DbContext
builder.Services.AddDbContext<CoastalHomestayDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//================================

// ========== SERVICES ==========

builder.Services.AddScoped<IAuthService, CHMS.BLL.Services.Implementations.AuthService>();
builder.Services.AddScoped<IHomestayService, HomestayService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<CHMS.DAL.Common.IUnitOfWork, CHMS.DAL.Common.UnitOfWork>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CHMS.BLL.Mappings.MappingProfile>();
});
builder.Services.AddMemoryCache();
builder.Services.AddScoped<EmailService>();
// Database
builder.Services.AddDatabase(builder.Configuration);

// Repositories
builder.Services.AddRepositories();

// Services (BLL)
builder.Services.AddServices();

// Third Party Services
builder.Services.AddThirdPartyServices(builder.Configuration);

// AutoMapper
//builder.Services.AddAutoMapperProfiles();

// CORS
builder.Services.AddCorsPolicy(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerDocumentation();

// SignalR
builder.Services.AddSignalRHubs();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// 2. Cấu hình Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE ==========

// Exception handling (đặt đầu tiên để bắt tất cả exceptions)
//app.UseExceptionMiddleware();

// Swagger (chỉ trong Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Coastal Homestay API v1");
        options.RoutePrefix = "swagger";
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
