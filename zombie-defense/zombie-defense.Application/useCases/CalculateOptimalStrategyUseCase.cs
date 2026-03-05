using zombie_defense.Application.DTOs;
using zombie_defense.Domain.Ports;

namespace zombie_defense.Application.useCases
{
    public class CalculateOptimalStrategyUseCase(IZombieTypeRepository zombieTypeRepository)
    {

        public async Task<StrategyResult> ExecuteAsync(int bullets, int seconds)
        {

            var zombies = (await zombieTypeRepository.GetAllAsync()).ToList();
            if (zombies.Count == 0 || bullets == 0 || seconds == 0)
                return new StrategyResult { TotalScore = 0, BulletsUsed = bullets, SecondsUsed = seconds };


            // generar Unbounded Knapsack 2D

            return new StrategyResult
            {
                TotalScore = 0,
                BulletsUsed = bullets,
                SecondsUsed = seconds
            };

        }
    }
}
