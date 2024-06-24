using DAL.Abstract;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class MarketDataRepository : GenericRepository<MarketData>, IMarketDataRepository
    {
        public MarketDataRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<MarketData[]> GetHistoricalMarketData(string instrumentalId)
        {
            return await _context.MarketData.Where(x => x.InstrumentId == instrumentalId).OrderByDescending(x => x.Timestamp).ToArrayAsync();
        }

        public async Task<double?> GetLatestPrice(string instrumentalId)
        {
            return await _context.MarketData.Where(x => x.InstrumentId == instrumentalId && x.Kind == "Last").OrderByDescending(x => x.Timestamp).Select(x => x.Price).FirstOrDefaultAsync();
        }
    }
}
