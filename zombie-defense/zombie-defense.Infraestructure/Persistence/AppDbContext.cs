using Microsoft.EntityFrameworkCore;
using zombie_defense.Domain.Entities;

namespace zombie_defense.Infraestructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ZombieType> ZombieTypes { get; set; }
        public DbSet<Simulation> Simulations { get; set; }
        public DbSet<EliminatedZombie> EliminatedZombies { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── ZombieTypes ──────────────────────────────────────────
            modelBuilder.Entity<ZombieType>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(e => e.Type)
                      .IsUnique();

                entity.Property(e => e.ThreatLevel)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_ZombieTypes_ShootingTime", "[ShootingTime] > 0");
                    t.HasCheckConstraint("CK_ZombieTypes_BulletsNeeded", "[BulletsNeeded] > 0");
                    t.HasCheckConstraint("CK_ZombieTypes_Score", "[Score] > 0");
                    t.HasCheckConstraint("CK_ZombieTypes_ThreatLevel",
                        "[ThreatLevel] IN ('HIGH', 'MEDIUM', 'LOW')");
                });
            });

            // ── Simulations ──────────────────────────────────────────
            modelBuilder.Entity<Simulation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Date)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.TotalScore)
                      .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.ToTable(t =>
                {
                    // Trigger activo: desactivar OUTPUT clause para compatibilidad
                    t.UseSqlOutputClause(false);

                    t.HasCheckConstraint("CK_Simulations_TimeAvailable", "[TimeAvailable] > 0");
                    t.HasCheckConstraint("CK_Simulations_BulletsAvailable", "[BulletsAvailable] > 0");
                });
            });

            // ── EliminatedZombies ─────────────────────────────────────
            modelBuilder.Entity<EliminatedZombie>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Timestamp)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.ZombieType)
                      .WithMany(z => z.EliminatedZombies)
                      .HasForeignKey(e => e.ZombieTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Simulation)
                      .WithMany(s => s.EliminatedZombies)
                      .HasForeignKey(e => e.SimulationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── AuditLog ──────────────────────────────────────────────
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TableName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Operation)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(e => e.OldData)
                      .HasColumnType("nvarchar(max)");

                entity.Property(e => e.NewData)
                      .HasColumnType("nvarchar(max)");

                entity.Property(e => e.DbUser)
                      .HasMaxLength(100)
                      .HasDefaultValueSql("SYSTEM_USER");

                entity.Property(e => e.AppUser)
                      .HasMaxLength(100);

                entity.Property(e => e.Timestamp)
                      .HasDefaultValueSql("GETDATE()");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_AuditLog_Operation",
                        "[Operation] IN ('INSERT', 'UPDATE', 'DELETE')");
                });
            });
        }
    }
}
