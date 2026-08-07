namespace StockMarketLive.Api.Services;

using Microsoft.AspNetCore.SignalR;
using StockMarketLive.Api.Hubs;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Domain.Constants;
using StockMarket.Shared.Contracts.Events;

/// <summary>
/// Application katmanının dış dünyaya (SignalR) fırlatacağı verileri sarmalayan servis.
/// Api projesinde (Presentation Layer) implemente edilmiştir (Clean Architecture yönü için).
/// </summary>
public sealed class SignalRLiveStockService(IHubContext<StockHub> hubContext) : ILiveStockService
{
    public async Task BroadcastStockUpdateAsync(AnalysisInfoPublishedEvent stockEvent, CancellationToken ct)
    {
        // Tüm bağlı client'lara event gönderiliyor.
        await hubContext.Clients.All.SendAsync(
            AppConstants.SignalR.ReceiveEventName, 
            stockEvent, 
            cancellationToken: ct);
    }

    public async Task BroadcastOrderCreatedAsync(OrderCreatedEvent orderEvent, CancellationToken ct)
    {
        await hubContext.Clients.All.SendAsync(
            AppConstants.SignalR.ReceiveOrderCreated, 
            orderEvent, 
            cancellationToken: ct);
    }

    public async Task BroadcastStockPriceUpdatedAsync(StockPriceUpdatedEvent priceEvent, CancellationToken ct)
    {
        await hubContext.Clients.All.SendAsync(
            AppConstants.SignalR.ReceiveStockPriceUpdated, 
            priceEvent, 
            cancellationToken: ct);
    }
}
