namespace zombie_defense.Domain.Entities
{
    public class Simulation
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TimeAvailable { get; set; }
        public int BulletsAvailable { get; set; }
        public int TotalScore { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<EliminatedZombie> EliminatedZombies { get; set; } = new List<EliminatedZombie>();
    }
}