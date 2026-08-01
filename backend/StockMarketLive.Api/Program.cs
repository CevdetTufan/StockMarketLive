using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockMarketLive.Api.Endpoints;
using StockMarketLive.Api.Extensions;
using StockMarketLive.Api.Hubs;
using StockMarketLive.Api.Services;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Constants;
using StockMarketLive.Infrastructure;
using System.Text;

using StockMarketLive.Application.Settings;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORS Konfigürasyonu ---
var corsSettings = new CorsSettings();
builder.Configuration.GetSection(CorsSettings.SectionName).Bind(corsSettings);

builder.Services.AddCors(options =>
{
    options.AddPolicy(AppConstants.CorsPolicyName, policy =>
    {
        policy.WithOrigins(corsSettings.Origins)
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

// --- 5. Veritabanı (Data Seeding) ---
await app.InitializeDatabaseAsync();

// --- 6. Minimal API Endpoints ---
app.MapAuthEndpoints();

app.MapHub<StockHub>(AppConstants.SignalR.HubEndpoint);

await app.RunAsync();
