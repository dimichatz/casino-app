using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Data
{
    public class CasinoAppBackendDbContext : DbContext
    {
        public CasinoAppBackendDbContext()
        {

        }

        public CasinoAppBackendDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<KycDocument> KycDocuments { get; set; }
        public DbSet<PlayerDetailsAudit> PlayerDetailsAudits { get; set; }
        public DbSet<PlayerSelfExclusionAudit> PlayerSelfExclusionAudits { get; set; }
        public DbSet<PlayerBanAudit> PlayerBanAudits { get; set; }
        public DbSet<PlayerLimitAudit> PlayerLimitAudits { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<PlayerLimit> PlayerLimits { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }

        public override int SaveChanges()
        {
            ApplyTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTimestamps()
        {
            var utcNow = DateTimeOffset.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                // Creatable: InsertedAt always set on ADD
                if (entry.Entity is CreatableEntity creatable)
                {
                    if (entry.State == EntityState.Added && creatable.InsertedAt == default)
                    {
                        creatable.InsertedAt = utcNow;
                    }
                }

                // Modifiable: needs InsertedAt + ModifiedAt
                if (entry.Entity is ModifiableEntity modifiable)
                {
                    // On creation: set both
                    if (entry.State == EntityState.Added)
                    {
                        if (modifiable.InsertedAt == default)
                            modifiable.InsertedAt = utcNow;

                        modifiable.ModifiedAt = utcNow;
                    }

                    // On update: set ModifiedAt
                    if (entry.State == EntityState.Modified)
                    {
                        modifiable.ModifiedAt = utcNow;

                        entry.Property(nameof(ModifiableEntity.ModifiedAt)).IsModified = true;
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(20);
                entity.Property(e => e.Password).HasMaxLength(60);
                entity.Property(e => e.Email).HasMaxLength(60);
                entity.Property(e => e.PhoneNumber).HasMaxLength(25);
                entity.Property(e => e.Firstname).HasMaxLength(60);
                entity.Property(e => e.Lastname).HasMaxLength(60);
                entity.Property(e => e.UserRole).HasConversion<string>();
                entity.Property(e => e.IsActive);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admins");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Admins_Users");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BirthDate);
                entity.Property(e => e.Gender).HasConversion<string>();
                entity.Property(e => e.StreetName).HasMaxLength(100);
                entity.Property(e => e.StreetNumber).HasMaxLength(20);
                entity.Property(e => e.PostalCode).HasMaxLength(10);
                entity.Property(e => e.City).HasMaxLength(60);
                entity.Property(e => e.IsAgeVerified);
                entity.Property(e => e.HasAcceptedTerms);
                entity.Property(e => e.IsKycVerified);
                entity.Property(e => e.IsSelfExcluded);
                entity.Property(e => e.SelfExclusionStart);
                entity.Property(e => e.SelfExclusionEnd);
                entity.Property(e => e.SelfExclusionPeriod).HasConversion<string>();

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.User).WithOne(p => p.Player)
                .HasForeignKey<Player>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Players_Users");

                entity.HasOne(d => d.Country).WithMany(p => p.Players)
                .HasForeignKey(d => d.CountryCode)
                .HasPrincipalKey(p => p.CountryCode)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Players_Countries");

                entity.HasMany(entity => entity.PlayerDetailsAudits).WithOne(s => s.Player);
            });

            modelBuilder.Entity<PlayerLimit>(entity =>
            {
                entity.ToTable("PlayerLimits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DepositDailyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DepositWeeklyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DepositMonthlyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.LossDailyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.LossWeeklyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.LossMonthlyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingDepositDailyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingDepositWeeklyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingDepositMonthlyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingLossDailyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingLossWeeklyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingLossMonthlyLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PendingDepositDailyLimitStart);
                entity.Property(e => e.PendingDepositWeeklyLimitStart);
                entity.Property(e => e.PendingDepositMonthlyLimitStart);
                entity.Property(e => e.PendingLossDailyLimitStart);
                entity.Property(e => e.PendingLossWeeklyLimitStart);
                entity.Property(e => e.PendingLossMonthlyLimitStart);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(e => e.PlayerId).IsUnique();

                entity.HasOne(d => d.Player).WithOne(p => p.PlayerLimit)
                .HasForeignKey<PlayerLimit>(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PlayerLimits_Players");
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(3);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(a => a.PlayerId).IsUnique();

                entity.HasOne(d => d.Player).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Accounts_Players");

                entity.HasMany(entity => entity.Transactions).WithOne(s => s.Account);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transactions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransactionType).HasConversion<string>();
                entity.Property(e => e.GameRoundId);
                entity.Property(e => e.TransactionNumber);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.TransactionStatus).HasMaxLength(15);
                entity.Property(e => e.OldBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NewBalance).HasColumnType("decimal(18,2)");

                entity.Property(e => e.InsertedAt).IsRequired();

                entity.HasIndex(e => new { e.AccountId, e.InsertedAt, e.TransactionType });
                entity.HasIndex(e => e.TransactionNumber).IsUnique();
                entity.HasIndex(e => e.GameRoundId);
                entity.HasIndex(e => e.InsertedAt);
                entity.HasIndex(e => e.GameId);

                entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transactions_Accounts");

                entity.HasOne(d => d.Game).WithMany()
                .HasForeignKey(t => t.GameId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transactions_Games");
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("Attachments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName);
                entity.Property(e => e.BlobName);
                entity.Property(e => e.ContentType).HasMaxLength(100);
                entity.Property(e => e.Extension).HasMaxLength(10);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(e => e.KycDocumentId);

                entity.HasOne(d => d.KycDocument).WithOne(p => p.Attachment)
                .HasForeignKey<Attachment>(d => d.KycDocumentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Attachments_KycDocuments");
            });

            modelBuilder.Entity<KycDocument>(entity =>
            {
                entity.ToTable("KycDocuments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DocumentType).HasConversion<string>();
                entity.Property(e => e.DocumentNumber).HasMaxLength(25);
                entity.Property(e => e.ExpireDate);
                entity.Property(e => e.KycStatus).HasConversion<string>();
                entity.Property(e => e.KycCheckDate);
                entity.Property(e => e.KycCheckedBy).HasMaxLength(60);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(e => e.PlayerId);

                entity.HasOne(d => d.Player).WithOne(p => p.KycDocument)
                .HasForeignKey<KycDocument>(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_KycDocuments_Players");
            });

            modelBuilder.Entity<PlayerDetailsAudit>(entity =>
            {
                entity.ToTable("PlayerDetailsAudits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChangedByUserId);
                entity.Property(e => e.ChangedByUsername).HasMaxLength(20);
                entity.Property(e => e.FieldName).HasMaxLength(60);
                entity.Property(e => e.OldValue).HasMaxLength(60);
                entity.Property(e => e.NewValue).HasMaxLength(60);
                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.InsertedAt).IsRequired();

                entity.HasIndex(e => e.PlayerId);

                entity.HasOne(d => d.Player).WithMany(p => p.PlayerDetailsAudits)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PlayerDetailsAudits_Players");
            });

            modelBuilder.Entity<PlayerSelfExclusionAudit>(entity =>
            {
                entity.ToTable("PlayerSelfExclusionAudits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SelfExclusionStart);
                entity.Property(e => e.SelfExclusionEnd);
                entity.Property(e => e.SelfExclusionPeriod).HasConversion<string>();

                entity.Property(e => e.InsertedAt).IsRequired();

                entity.HasIndex(e => e.PlayerId);

                entity.HasOne(d => d.Player).WithMany(p => p.PlayerSelfExclusionAudits)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PlayerSelfExclusionAudits_Players");
            });

            modelBuilder.Entity<PlayerBanAudit>(entity =>
            {
                entity.ToTable("PlayerBanAudits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsBanned);
                entity.Property(e => e.ChangedByUserId);
                entity.Property(e => e.ChangedByUsername).HasMaxLength(20);
                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.InsertedAt).IsRequired();

                entity.HasIndex(e => e.PlayerId);

                entity.HasOne(d => d.Player).WithMany(p => p.PlayerBanAudits)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PlayerBanAudits_Players");
            });

            modelBuilder.Entity<PlayerLimitAudit>(entity =>
            {
                entity.ToTable("PlayerLimitAudits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FieldName).HasMaxLength(60);
                entity.Property(e => e.OldLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NewLimit).HasColumnType("decimal(18,2)");

                entity.Property(e => e.InsertedAt).IsRequired();

                entity.HasIndex(e => e.PlayerId);

                entity.HasOne(d => d.Player).WithMany(p => p.PlayerLimitAudits)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PlayerLimitAudits_Players");
            });

            modelBuilder.Entity<AppSetting>(entity =>
            {
                entity.ToTable("AppSettings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).HasMaxLength(100);
                entity.Property(e => e.Value).HasMaxLength(100);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(e => e.Key).IsUnique();
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Countries");
                entity.HasKey(e => e.CountryCode);
                entity.Property(e => e.CountryCode).HasMaxLength(2);
                entity.Property(e => e.CountryName).HasMaxLength(60);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Games");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(30);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
                entity.Property(e => e.IsEnabled);
                entity.Property(e => e.MinBet).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MaxBet).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).HasMaxLength(60);

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.ToTable("GameSessions");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExternalSessionId);
                entity.Property(e => e.GameStatus).HasConversion<string>();

                entity.Property(e => e.InsertedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                entity.HasOne(e => e.User).WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_GameSessions_Users");

                entity.HasOne(e => e.Game).WithMany()
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_GameSessions_Games");

                entity.HasIndex(e => e.ExternalSessionId).IsUnique();
                entity.HasIndex(e => e.UserId);
            });

            // Define the transaction number sequence for generating TransactionNumbers
            modelBuilder.HasSequence<long>("TransactionNumberSequence")
                .StartsAt(100000)
                .IncrementsBy(1);
        }
    }
}
