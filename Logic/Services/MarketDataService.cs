using DAL.Abstract;
using DAL.Entities;
using Logic.Dtos;
using Logic.Interfaces;
using Newtonsoft.Json;

namespace Logic.Services
{
    public class MarketDataService(IMarketDataRepository marketDataRepository) : IMarketDataService
    {
        public async Task HandleMarketData(string message)
        {
            var marketDataResponse = JsonConvert.DeserializeObject<MarketDataUpdatedResponse>(message);

            MarketData? marketData = null;
            if (marketDataResponse?.Ask != null)
            {
                marketData = new MarketData
                {
                    Type = marketDataResponse.Type,
                    InstrumentId = marketDataResponse.InstrumentId,
                    Provider = marketDataResponse.Provider,
                    Kind = "Ask",
                    Timestamp = marketDataResponse.Ask.Timestamp,
                    Price = marketDataResponse.Ask.Price,
                    Volume = marketDataResponse.Ask.Volume
                };
            }

            if (marketDataResponse?.Bid != null)
            {
                marketData = new MarketData
                {
                    Type = marketDataResponse.Type,
                    InstrumentId = marketDataResponse.InstrumentId,
                    Provider = marketDataResponse.Provider,
                    Kind = "Bid",
                    Timestamp = marketDataResponse.Bid.Timestamp,
                    Price = marketDataResponse.Bid.Price,
                    Volume = marketDataResponse.Bid.Volume
                };
            }

            if (marketDataResponse?.Last != null)
            {
                marketData = new MarketData
                {
                    Type = marketDataResponse.Type,
                    InstrumentId = marketDataResponse.InstrumentId,
                    Provider = marketDataResponse.Provider,
                    Kind = "Last",
                    Timestamp = marketDataResponse.Last.Timestamp,
                    Price = marketDataResponse.Last.Price,
                    Volume = marketDataResponse.Last.Volume
                };
            }

            if (marketData != null)
            {
                await marketDataRepository.AddAsync(marketData);
            }
        }

        public async Task<MarketData[]> GetHistoricalMarketData(string instrumentalId)
        {
            return await marketDataRepository.GetHistoricalMarketData(instrumentalId);
        }

        public async Task<double?> GetLatestPrice(string instrumentalId)
        {
            return await marketDataRepository.GetLatestPrice(instrumentalId);
        }
    }
}
