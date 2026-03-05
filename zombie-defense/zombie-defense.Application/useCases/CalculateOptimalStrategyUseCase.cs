using zombie_defense.Application.DTOs;
using zombie_defense.Domain.Entities;
using zombie_defense.Domain.Ports;

namespace zombie_defense.Application.useCases
{
    public class CalculateOptimalStrategyUseCase(IZombieTypeRepository zombieTypeRepository)
    {

        public async Task<StrategyResult> ExecuteAsync(int bullets, int seconds)
        {

            var zombies = (await zombieTypeRepository.GetAllAsync()).ToList();
            var countZombies = zombies.Count;
            if (countZombies == 0 || bullets == 0 || seconds == 0)
                return new StrategyResult { TotalScore = 0, BulletsUsed = bullets, SecondsUsed = seconds };


            // generar Unbounded Knapsack 2D
            // crear una matriz de tamaño (bullets + 1) x (seconds + 1)
            var dp = new int[bullets + 1, seconds + 1];

            for (int i = 0; i < countZombies; i++)
            {
                var zombie = zombies[i];
                // iterar desde el principio hasta el número de balas y segundos disponibles
                for (int b = zombie.BulletsNeeded; b <= bullets; b++)
                {
                    for (int s = zombie.ShootingTime; s <= seconds; s++)
                    {
                        // validar si el zombie puede ser eliminado con las balas y segundos disponibles
                        int scoreIfKilled = dp[b - zombie.BulletsNeeded, s - zombie.ShootingTime] + zombie.Score;
                        if (scoreIfKilled > dp[b, s])
                        {
                            dp[b, s] = scoreIfKilled;
                        }
                    }
                }
            }

            //Reconstrucción del subconjunto - necesita rastrear las eliminaciones
            var selectedKills = ReconstructSolution(zombies, dp, bullets, seconds);

            var eliminatedZombies = EliminatedZombiesOrdered(selectedKills);

            return new StrategyResult
            {
                TotalScore = selectedKills.Sum(z => z.Score),
                BulletsUsed = selectedKills.Sum(z => z.BulletsNeeded),
                SecondsUsed = selectedKills.Sum(z => z.ShootingTime),
                EliminatedZombies = eliminatedZombies
            };

        }

        private static List<ZombieType> ReconstructSolution(List<ZombieType> zombieTypes, int[,] dp, int bullets, int seconds)
        {
            {
                var selectedZombies = new List<ZombieType>();
                int b = bullets, s = seconds;

                while (dp[b, s] > 0)
                {
                    bool found = false;
                    foreach (var zombie in zombieTypes)
                    {
                        if (b >= zombie.BulletsNeeded && s >= zombie.ShootingTime)
                        {
                            int scoreIfKilled = dp[b - zombie.BulletsNeeded, s - zombie.ShootingTime] + zombie.Score;
                            if (scoreIfKilled == dp[b, s])
                            {
                                selectedZombies.Add(zombie);
                                b -= zombie.BulletsNeeded;
                                s -= zombie.ShootingTime;
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found) break;
                }

                return selectedZombies;

            }
        }

        private static List<ZombieKillSummary> EliminatedZombiesOrdered(List<ZombieType> selectedZombies)
        {
            return [.. selectedZombies
                .GroupBy(z => z.Id)
                .Select(g => new ZombieKillSummary
                {
                    ZombieTypeId = g.Key,
                    Type = g.First().Type,
                    BulletsNeeded = g.First().BulletsNeeded,
                    ShootingTime = g.First().ShootingTime,
                    Score = g.First().Score,
                    ThreatLevel = g.First().ThreatLevel,
                    KillCount = g.Count()
                })
                .OrderBy(z => z.ThreatLevel switch
                {
                    "HIGH" => 1,
                    "MEDIUM" => 2,
                    _ => 3
                })];
        }
    }
}
