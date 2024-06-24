using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssetsController(IFintachartsApiService fintachartsApiService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string provider, [FromQuery] string kind)
        {
            return Ok(await fintachartsApiService.GetAssets(provider, kind));
        }
    }
}