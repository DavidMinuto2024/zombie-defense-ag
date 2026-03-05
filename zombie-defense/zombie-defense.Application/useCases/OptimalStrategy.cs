using zombie_defense.Application.DTOs;
using zombie_defense.Domain.Entities;

namespace zombie_defense.Application.useCases
{
    public class OptimalStrategy(CalculateOptimalStrategyUseCase calculateStrategy, SaveSimulationUseCase saveSimulation)
    {
        public async Task<StrategyResult> ExecuteAsync(int bullets, int seconds)
        {
            var strategyResult = await calculateStrategy.ExecuteAsync(bullets, seconds);
            var simulation = new Simulation
            {
                BulletsAvailable = bullets,
                TimeAvailable = seconds,
                TotalScore = strategyResult.TotalScore,
                Date = DateTime.UtcNow,
                EliminatedZombies = [.. strategyResult.EliminatedZombies.Select(z => new EliminatedZombie
                        {
                            ZombieTypeId = z.ZombieTypeId,
                            PointsEarned = z.Score,
                            Timestamp = DateTime.UtcNow
                    })]
            };
            await saveSimulation.ExecuteAsync(simulation);


            return strategyResult;
        }
    }
}
