using StockMarketLive.Domain.Entities;

namespace StockMarketLive.Application.Interfaces;

public interface IJwtProvider
{
    string Generate(User user);
}
