using OpenTelemetry.Logs;
using StockMarketLive.Api.Endpoints;
using StockMarketLive.Api.Extensions;
using StockMarketLive.Api.Hubs;
using StockMarketLive.Api.Services;
using StockMarketLive.Application;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Application.Settings;
using StockMarketLive.Domain.Constants;
using StockMarketLive.Infrastructure;

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

// --- 3. Application & Infrastructure Kayıtları ---
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// --- 4. JWT Authentication (Güvenlik Kuralı) ---
builder.Services.AddApiAuthentication(builder.Configuration);

// --- 5. Global Exception Handler (Goal 11) ---
builder.Services.AddExceptionHandler<StockMarketLive.Api.Middlewares.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- 6. OpenTelemetry & Grafana Cloud (Observability) ---
builder.Services.AddApiTelemetry(builder.Configuration);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
    
    var telemetrySettings = new TelemetrySettings();
    builder.Configuration.GetSection(TelemetrySettings.SectionName).Bind(telemetrySettings);

    if (!string.IsNullOrEmpty(telemetrySettings.OtlpEndpoint))
    {
        logging.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(telemetrySettings.OtlpEndpoint);
            if (!string.IsNullOrEmpty(telemetrySettings.OtlpHeaders))
            {
                options.Headers = telemetrySettings.OtlpHeaders;
            }
        });
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseCors(AppConstants.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

// --- 5. Veritabanı (Data Seeding) ---
await app.InitializeDatabaseAsync();

// --- 6. Minimal API Endpoints ---
app.MapAuthEndpoints();

app.MapHub<StockHub>(AppConstants.SignalR.HubEndpoint);

await app.RunAsync();
