using ASGShared.Models;
using Microsoft.EntityFrameworkCore;

namespace ASGBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<MealPlan> MealPlans { get; set; } = null!;
        public DbSet<MealPlanRecipe> MealPlanRecipes { get; set; } = null!;
        public DbSet<Recipe> Recipes { get; set; } = null!;
        public DbSet<UserPreferences> UserPreferences { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.BudgetPerMeal);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                 .HasOne(u => u.Preferences)
                 .WithOne()
                 .HasForeignKey<UserPreferences>(up => up.UserId)
                 .IsRequired();

            modelBuilder.Entity<UserPreferences>()
                .Property(up => up.Allergies).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            modelBuilder.Entity<UserPreferences>()
                .Property(up => up.FavoriteCuisines).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            
            modelBuilder.Entity<UserPreferences>()
                .OwnsOne(up => up.DietaryRestrictions);

            modelBuilder.Entity<UserPreferences>()
                .OwnsOne(up => up.NutritionalGoals);
        }
    }
}
