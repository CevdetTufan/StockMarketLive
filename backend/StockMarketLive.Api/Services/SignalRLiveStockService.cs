namespace StockMarketLive.Api.Services;

using Microsoft.AspNetCore.SignalR;
using StockMarketLive.Api.Hubs;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Constants;
using StockMarketLive.Domain.Events;

/// <summary>
/// Application katmanının dış dünyaya (SignalR) fırlatacağı verileri sarmalayan servis.
/// Api projesinde (Presentation Layer) implemente edilmiştir (Clean Architecture yönü için).
/// </summary>
public sealed class SignalRLiveStockService(IHubContext<StockHub> hubContext) : ILiveStockService
{
    public async Task BroadcastStockUpdateAsync(StockPriceAnalyzedEvent stockEvent, CancellationToken ct)
    {
        // Tüm bağlı client'lara event gönderiliyor.
        await hubContext.Clients.All.SendAsync(
            AppConstants.SignalR.ReceiveEventName, 
            stockEvent, 
            cancellationToken: ct);
    }
}
