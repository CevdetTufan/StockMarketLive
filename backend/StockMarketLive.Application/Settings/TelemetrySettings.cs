namespace StockMarketLive.Application.Settings;

public class TelemetrySettings
{
    public const string SectionName = "Telemetry";

    public string ServiceName { get; set; } = "StockMarketLive.Api";
    public string OtlpEndpoint { get; set; } = string.Empty;
    public string OtlpHeaders { get; set; } = string.Empty;
}
