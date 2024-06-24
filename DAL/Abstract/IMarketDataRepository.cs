using DAL.Entities;

namespace DAL.Abstract
{
    public interface IMarketDataRepository : IGenericRepository<MarketData>
    {
        Task<MarketData[]> GetHistoricalMarketData(string instrumentalId);

        Task<double?> GetLatestPrice(string instrumentalId);
    }
}
