namespace Logic.Dtos;

public class Paging
{
    public int Page { get; set; }
    public int Pages { get; set; }
    public int Items { get; set; }
}

public class Mapping
{
    public string Symbol { get; set; }
    public string Exchange { get; set; }
    public int DefaultOrderSize { get; set; }
}

public class Mappings
{
    public Mapping ActiveTick { get; set; }
    public Mapping Simulation { get; set; }
    public Mapping Oanda { get; set; }
}

public class ForexData
{
    public string Id { get; set; }
    public string Symbol { get; set; }
    public string Kind { get; set; }
    public string Description { get; set; }
    public decimal TickSize { get; set; }
    public string Currency { get; set; }
    public string BaseCurrency { get; set; }
    public Mappings Mappings { get; set; }
}

public class AssetsResponse
{
    public Paging Paging { get; set; }
    public List<ForexData> Data { get; set; }
}
