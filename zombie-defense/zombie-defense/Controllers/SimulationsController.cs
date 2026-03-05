using Microsoft.AspNetCore.Mvc;
using zombie_defense.Application.useCases;

namespace zombie_defense.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationsController(HistoryUseCase historyUseCase) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await historyUseCase.ExecuteAsync();
            return Ok(result);
        }
    }
}
