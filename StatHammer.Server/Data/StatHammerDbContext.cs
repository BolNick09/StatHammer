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
        }
    }
}
