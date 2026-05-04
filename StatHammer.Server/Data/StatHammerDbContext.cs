using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Data
{
    public class StatHammerDbContext : DbContext
    {
        public StatHammerDbContext(DbContextOptions<StatHammerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<Ability> Abilities { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Wargear> Wargears { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<WeaponProfile> WeaponProfiles { get; set; }
        public DbSet<WeaponProfileAbility> WeaponProfileAbilities { get; set; }

        public DbSet<Model> Models { get; set; }
        public DbSet<ModelWeapon> ModelWeapons { get; set; }
        public DbSet<ModelWargear> ModelWargears { get; set; }
        public DbSet<ModelAbility> ModelAbilities { get; set; }

        public DbSet<UnitModel> UnitModels { get; set; }
        public DbSet<UnitAbility> UnitAbilities { get; set; }
        public DbSet<UnitKeyword> UnitKeywords { get; set; }
        public DbSet<UnitOption> UnitOptions { get; set; }
        public DbSet<OptionItem> OptionItems { get; set; }


        public DbSet<SimulationResult> SimulationResults { get; set; }
        public DbSet<TurnStat> TurnStats { get; set; }
        public DbSet<WeaponStat> WeaponStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Ability>(entity =>
            {
                entity.ToTable("Abilities");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Keyword>(entity =>
            {
                entity.ToTable("Keywords");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Wargear>(entity =>
            {
                entity.ToTable("Wargears");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<Weapon>(entity =>
            {
                entity.ToTable("Weapons");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<WeaponProfile>(entity =>
            {
                entity.ToTable("WeaponProfiles");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Attacks)
                    .IsRequired();

                entity.Property(e => e.Damage)
                    .IsRequired();

                entity.HasOne(e => e.Weapon)
                    .WithMany(w => w.WeaponProfiles)
                    .HasForeignKey(e => e.WeaponId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WeaponProfileAbility>(entity =>
            {
                entity.ToTable("WeaponProfileAbilities");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.WeaponProfile)
                    .WithMany(wp => wp.WeaponProfileAbilities)
                    .HasForeignKey(e => e.WeaponProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Ability)
                    .WithMany()
                    .HasForeignKey(e => e.AbilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.WeaponProfileId, e.AbilityId })
                    .IsUnique();
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("Models");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.Move)
                    .IsRequired();

                entity.Property(e => e.Toughness)
                    .IsRequired();

                entity.Property(e => e.Save)
                    .IsRequired();

                entity.Property(e => e.Wounds)
                    .IsRequired();

                entity.Property(e => e.Leadership)
                    .IsRequired();

                entity.Property(e => e.OC)
                    .IsRequired();
            });

            modelBuilder.Entity<ModelWeapon>(entity =>
            {
                entity.ToTable("ModelWeapons");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.IsDefault)
                    .IsRequired();

                entity.HasOne(e => e.Model)
                    .WithMany(m => m.ModelWeapons)
                    .HasForeignKey(e => e.ModelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Weapon)
                    .WithMany(w => w.ModelWeapons)
                    .HasForeignKey(e => e.WeaponId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ModelId, e.WeaponId })
                    .IsUnique();
            });

            modelBuilder.Entity<ModelWargear>(entity =>
            {
                entity.ToTable("ModelWargears");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.IsDefault)
                    .IsRequired();

                entity.HasOne(e => e.Model)
                    .WithMany(m => m.ModelWargears)
                    .HasForeignKey(e => e.ModelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Wargear)
                    .WithMany(w => w.ModelWargears)
                    .HasForeignKey(e => e.WargearId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ModelId, e.WargearId })
                    .IsUnique();
            });

            modelBuilder.Entity<ModelAbility>(entity =>
            {
                entity.ToTable("ModelAbilities");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Model)
                    .WithMany(m => m.ModelAbilities)
                    .HasForeignKey(e => e.ModelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Ability)
                    .WithMany()
                    .HasForeignKey(e => e.AbilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ModelId, e.AbilityId })
                    .IsUnique();
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();
            });

            modelBuilder.Entity<UnitModel>(entity =>
            {
                entity.ToTable("UnitModels");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.MinCount)
                    .IsRequired();

                entity.Property(e => e.MaxCount)
                    .IsRequired();

                entity.HasOne(e => e.Unit)
                    .WithMany(u => u.UnitModels)
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Model)
                    .WithMany()
                    .HasForeignKey(e => e.ModelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UnitId, e.ModelId })
                    .IsUnique();
            });

            modelBuilder.Entity<UnitAbility>(entity =>
            {
                entity.ToTable("UnitAbilities");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Unit)
                    .WithMany(u => u.UnitAbilities)
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Ability)
                    .WithMany()
                    .HasForeignKey(e => e.AbilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UnitId, e.AbilityId })
                    .IsUnique();
            });

            modelBuilder.Entity<UnitKeyword>(entity =>
            {
                entity.ToTable("UnitKeywords");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Unit)
                    .WithMany(u => u.UnitKeywords)
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Keyword)
                    .WithMany()
                    .HasForeignKey(e => e.KeywordId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UnitId, e.KeywordId })
                    .IsUnique();
            });

            modelBuilder.Entity<UnitOption>(entity =>
            {
                entity.ToTable("UnitOptions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.MaxSelections)
                    .IsRequired();

                entity.Property(e => e.GroupKey);

                entity.HasOne(e => e.Unit)
                    .WithMany(u => u.UnitOptions)
                    .HasForeignKey(e => e.UnitId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OptionItem>(entity =>
            {
                entity.ToTable("OptionItems");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.UnitOption)
                    .WithMany(uo => uo.OptionItems)
                    .HasForeignKey(e => e.UnitOptionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Weapon)
                    .WithMany()
                    .HasForeignKey(e => e.WeaponId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Wargear)
                    .WithMany()
                    .HasForeignKey(e => e.WargearId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SimulationResult>(entity =>
            {
                entity.ToTable("SimulationResults");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.SimulationCount)
                    .IsRequired();

                entity.Property(e => e.CreatedAtUtc)
                    .IsRequired();

                entity.HasOne(e => e.UnitA)
                    .WithMany()
                    .HasForeignKey(e => e.UnitAId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UnitB)
                    .WithMany()
                    .HasForeignKey(e => e.UnitBId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TurnStat>(entity =>
            {
                entity.ToTable("TurnStats");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TurnNumber)
                    .IsRequired();

                entity.Property(e => e.Side)
                    .IsRequired();

                entity.Property(e => e.AvgModelsAlive)
                    .IsRequired();

                entity.Property(e => e.AvgWoundsAlive)
                    .IsRequired();

                entity.Property(e => e.AvgHits)
                    .IsRequired();

                entity.Property(e => e.AvgWounds)
                    .IsRequired();

                entity.Property(e => e.AvgSuccessfulSaves)
                    .IsRequired();

                entity.Property(e => e.AvgBlockedByFnp)
                    .IsRequired();

                entity.HasOne(e => e.SimulationResult)
                    .WithMany(sr => sr.TurnStats)
                    .HasForeignKey(e => e.SimulationResultId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.SimulationResultId, e.TurnNumber, e.Side })
                    .IsUnique();
            });

            modelBuilder.Entity<WeaponStat>(entity =>
            {
                entity.ToTable("WeaponStats");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AvgHits)
                    .IsRequired();

                entity.Property(e => e.AvgWounds)
                    .IsRequired();

                entity.Property(e => e.AvgDamage)
                    .IsRequired();

                entity.HasOne(e => e.TurnStat)
                    .WithMany(ts => ts.WeaponStats)
                    .HasForeignKey(e => e.TurnStatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Weapon)
                    .WithMany()
                    .HasForeignKey(e => e.WeaponId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.TurnStatId, e.WeaponId })
                    .IsUnique();
            });
        }
    }
}
