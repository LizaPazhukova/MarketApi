namespace DAL.Entities
{
    public class MarketData 
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string InstrumentId { get; set; }
        public string Provider { get; set; }
        public string Kind { get; set; }
        public DateTime Timestamp { get; set; }
        public double Price { get; set; }
        public int Volume { get; set; }
    }
}
