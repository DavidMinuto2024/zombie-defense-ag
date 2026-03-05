namespace zombie_defense.Application.DTOs
{
    public class ZombieKillSummary
    {
        public int ZombieTypeId { get; set; }
        public string Type { get; set; } = null!;
        public int BulletsNeeded { get; set; }
        public int ShootingTime { get; set; }
        public int Score { get; set; }
        public string ThreatLevel { get; set; } = null!;
        public int KillCount { get; set; }
    }
}
