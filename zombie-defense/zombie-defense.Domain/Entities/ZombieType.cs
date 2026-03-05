namespace zombie_defense.Domain.Entities
{
    public class ZombieType
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public int ShootingTime { get; set; }
        public int BulletsNeeded { get; set; }
        public int Score { get; set; }
        public string ThreatLevel { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<EliminatedZombie> EliminatedZombies { get; set; } = new List<EliminatedZombie>();
    }
}
