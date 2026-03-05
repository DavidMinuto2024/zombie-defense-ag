namespace zombie_defense.Application.DTOs
{
    public class StrategyResult
    {
        public int TotalScore { get; set; }
        public int BulletsUsed { get; set; }
        public int SecondsUsed { get; set; }

        public List<ZombieKillSummary> EliminatedZombies { get; set; } = [];
    }
}
