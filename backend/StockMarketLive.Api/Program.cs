using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockMarketLive.Api.Hubs;
using StockMarketLive.Api.Services;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Constants;
using StockMarketLive.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORS Konfigürasyonu ---
// Frontend (React/Vite) uygulamamızın SignalR'a bağlanabilmesi için gereklidir.
builder.Services.AddCors(options =>
{
    options.AddPolicy(AppConstants.CorsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Frontend portları
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // SignalR için gereklidir
    });
});

// --- 2. SignalR ve Özel Servis Kayıtları ---
builder.Services.AddSignalR();
// Clean Architecture: Interface uygulama katmanında, implementasyonu API katmanında.
builder.Services.AddSingleton<ILiveStockService, SignalRLiveStockService>();

// --- 3. Altyapı (Infrastructure) Kayıtları (MassTransit / RabbitMQ) ---
builder.Services.AddInfrastructureServices(builder.Configuration);

// --- 4. JWT Authentication (Güvenlik Kuralı) ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing");
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };

        // SignalR için WebSockets üzerinden gelen Token'ı Header yerine QueryString'den almak gerekir
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(AppConstants.SignalR.HubEndpoint))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(AppConstants.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

// Minimal API: Basit Login Endpointi (Mock) - Gerçekte DB'den doğrulanır.
app.MapPost("/api/auth/login", () =>
{
    // TODO: Gerçek Login mantığı
    return Results.Ok(new { Token = "dummy_token" });
});

app.MapHub<StockHub>(AppConstants.SignalR.HubEndpoint);

app.Run();
