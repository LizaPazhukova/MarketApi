using DAL.Abstract;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DalModuleDefinition
    {
        public static IServiceCollection AddDal(this IServiceCollection collection, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MARKET_DATA_DB_CONNECTION_STRING");

            collection.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            collection.AddScoped<IMarketDataRepository, MarketDataRepository>();

            return collection;
        }
    }
}
