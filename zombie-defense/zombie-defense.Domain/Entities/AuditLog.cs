namespace zombie_defense.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; } = null!;
        public string Operation { get; set; } = null!;
        public string? OldData { get; set; }
        public string? NewData { get; set; }
        public string? DbUser { get; set; }
        public string? AppUser { get; set; }
        public DateTime Timestamp { get; set; }
    }
}