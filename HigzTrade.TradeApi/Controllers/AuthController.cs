using HigzTrade.TradeApi.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json.Linq;

namespace HigzTrade.TradeApi.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting(RateLimitPolicy.AuthRateLimit)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            //todo: implement
            return Ok(new { token = "498489egrrwg498adsfgasdfgqer89wt89w3eqt897qw789et" });
        }
    }
}
    