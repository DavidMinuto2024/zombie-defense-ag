namespace zombie_defense.Domain.Entities
{
    public class EliminatedZombie
    {
        public int Id { get; set; }
        public int ZombieTypeId { get; set; }
        public int SimulationId { get; set; }
        public int PointsEarned { get; set; }
        public DateTime Timestamp { get; set; }

        public ZombieType ZombieType { get; set; } = null!;
        public Simulation Simulation { get; set; } = null!;
    }
}