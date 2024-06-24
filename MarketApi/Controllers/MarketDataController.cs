using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarketDataController(IMarketDataService marketDataService) : ControllerBase
    {
        [HttpGet]
        [Route("historicalMarketData")]
        public async Task<IActionResult> GetHistoricalMarketData([FromQuery] string instrumentalId)
        {
            return Ok(await marketDataService.GetHistoricalMarketData(instrumentalId));
        }

        [HttpGet]
        [Route("latestPrice")]
        public async Task<IActionResult> GetLatestPrice([FromQuery] string instrumentalId)
        {
            return Ok(await marketDataService.GetLatestPrice(instrumentalId));
        }
    }
}
