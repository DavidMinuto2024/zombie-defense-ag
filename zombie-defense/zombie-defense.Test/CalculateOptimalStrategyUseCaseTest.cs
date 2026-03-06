using FluentAssertions;
using Moq;
using zombie_defense.Application.useCases;
using zombie_defense.Domain.Entities;
using zombie_defense.Domain.Ports;

namespace zombie_defense.Test
{
    public class CalculateOptimalStrategyUseCaseTest
    {
        private static List<ZombieType> StandarZombies() =>
        [
         new() { Id = 1, Type = "Walker", BulletsNeeded = 2, ShootingTime = 3, Score = 10, ThreatLevel = "LOW" },
         new() { Id = 2, Type = "Runner", BulletsNeeded = 1, ShootingTime = 1, Score = 5, ThreatLevel = "MEDIUM" },
         new() { Id = 3, Type = "Brute", BulletsNeeded = 4, ShootingTime = 5, Score = 20, ThreatLevel = "HIGH" }
        ];

        public static CalculateOptimalStrategyUseCase BuildUseCase(IEnumerable<ZombieType> zombieTypes)
        {
            var mockRepository = new Mock<IZombieTypeRepository>();
            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(zombieTypes);
            return new CalculateOptimalStrategyUseCase(mockRepository.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsOptimalStrategy()
        {
            var useCase = BuildUseCase(StandarZombies());

            var result = await useCase.ExecuteAsync(5, 5);
            Assert.NotNull(result);
            Assert.Equal(25, result.TotalScore);

        }

        [Fact]
        public async Task ExecuteAsync_WithZeroBullets_ReturnsZeroScore()
        {
            var useCase = BuildUseCase(StandarZombies());
            var result = await useCase.ExecuteAsync(0, 5);
            result.TotalScore.Should().Be(0);
            result.EliminatedZombies.Should().BeEmpty();
        }
    }
}
