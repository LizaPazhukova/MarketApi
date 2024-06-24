using DAL.Entities;

namespace Logic.Interfaces
{
    public interface IMarketDataService
    {
        Task HandleMarketData(string message);

        Task<MarketData[]> GetHistoricalMarketData(string instrumentalId);

        Task<double?> GetLatestPrice(string instrumentalId);
    }
}
