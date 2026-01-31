using Microsoft.EntityFrameworkCore;

namespace HighLowGameApi.Data
{
    public class HighLowGameApiDbContext : DbContext
    {
        public HighLowGameApiDbContext()
        {

        }

        public HighLowGameApiDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<GameRound> GameRounds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.ToTable("GameSessions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StartCardSuit).HasConversion<string>();
                entity.Property(e => e.CurrentCardSuit).HasConversion<string>();
                entity.Property(e => e.GameStatus).HasConversion<string>();

                entity.Property(e => e.InsertedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<GameRound>(entity =>
            {
                entity.ToTable("GameRounds");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PreviousCardSuit).HasConversion<string>();
                entity.Property(e => e.GuessType).HasConversion<string>();
                entity.Property(e => e.NewCardSuit).HasConversion<string>();
                entity.Property(e => e.BetAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.WinAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.InsertedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.GameSessionId);

                entity.HasOne(d => d.GameSession).WithMany(p => p.GameRounds)
                .HasForeignKey(d => d.GameSessionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_GameRounds_GameSessions");
            });
        }
    }
}
