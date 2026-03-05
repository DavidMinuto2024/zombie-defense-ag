using Microsoft.AspNetCore.Mvc;
using zombie_defense.Application.useCases;

namespace zombie_defense.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DefenseController(OptimalStrategy optimalStrategy) : ControllerBase
    {
        [HttpGet("optimal-strategy")]
        public async Task<IActionResult> GetOptimalStrategy([FromQuery] int bullets, [FromQuery] int secondsAvailable)
        {
            if (bullets <= 0  || secondsAvailable <=0) {
                return BadRequest("Both 'bullets' and 'secondsAvailable' query parameters are required.");
            }

            var result = await optimalStrategy.ExecuteAsync(bullets, secondsAvailable);

            return Ok(result);
        }
    }
}
