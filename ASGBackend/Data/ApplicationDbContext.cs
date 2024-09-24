using ASGShared.Models;
using Microsoft.EntityFrameworkCore;

namespace ASGBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Users = Set<User>();
            UserPreferences = Set<UserPreferences>();
            DietaryRestrictions = Set<DietaryRestrictions>();
            DietaryRestrictionsUsers = Set<DietaryRestrictionsUser>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<DietaryRestrictions> DietaryRestrictions { get; set; }
        public DbSet<DietaryRestrictionsUser> DietaryRestrictionsUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.BudgetPerMeal, b =>
                {
                    b.Property(bm => bm.Amount).HasColumnType("decimal(18,2)"); // Specify precision and scale
                });

            modelBuilder.Entity<DietaryRestrictionsUser>()
                .HasKey(dru => new { dru.UserId, dru.DietaryRestrictionsId });

            modelBuilder.Entity<DietaryRestrictionsUser>()
                .HasOne(dru => dru.User)
                .WithMany(u => u.DietaryRestrictionsUsers)
                .HasForeignKey(dru => dru.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Specify ON DELETE NO ACTION

            modelBuilder.Entity<DietaryRestrictionsUser>()
                .HasOne(dru => dru.DietaryRestrictions)
                .WithMany()
                .HasForeignKey(dru => dru.DietaryRestrictionsId)
                .OnDelete(DeleteBehavior.NoAction); // Specify ON DELETE NO ACTION
        }
    }
}
