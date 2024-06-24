namespace Logic.Dtos
{
    public class MarketDataUpdatedResponse
    {
        public string Type { get; set; }
        public string InstrumentId { get; set; }
        public string Provider { get; set; }
        public MarketDataUpdatedDetails Ask { get; set; }
        public MarketDataUpdatedDetails Bid { get; set; }
        public MarketDataUpdatedDetails Last { get; set; }
    }

    public class MarketDataUpdatedDetails
    {
        public DateTime Timestamp { get; set; }
        public double Price { get; set; }
        public int Volume { get; set; }
    }
}
