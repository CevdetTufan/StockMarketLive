namespace StockMarketLive.Api.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

/// <summary>
/// Web client'ların bağlanacağı SignalR Hub.
/// [Authorize] ile sadece geçerli JWT Token'a sahip kişilerin veri akışını izlemesi garanti altına alınmıştır.
/// </summary>
[Authorize]
public sealed class StockHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Burada kullanıcı logları veya özel gruplara (Örn: "AAPL_Followers") atama yapılabilir.
        await base.OnConnectedAsync();
    }
}
