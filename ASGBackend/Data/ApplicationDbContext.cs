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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.BudgetPerMeal, b =>
                {
                    b.Property(bm => bm.Amount).HasColumnType("decimal(18,2)"); // Specify precision and scale
                });

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Preferences, p =>
                {
                    p.OwnsOne(up => up.DietaryRestrictions);
                    p.OwnsOne(up => up.NutritionalGoals);
                    p.Property(up => up.Allergies).HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                    p.Property(up => up.FavoriteCuisines).HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                    p.Property(up => up.DislikedFoods).HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                });

            modelBuilder.Entity<MealPlanRecipe>()
                .HasOne(mpr => mpr.Recipe)
                .WithMany()
                .HasForeignKey(mpr => mpr.RecipeId);

            modelBuilder.Entity<MealPlanRecipe>()
                .HasOne(mpr => mpr.MealPlan)
                .WithMany(mp => mp.Recipes)
                .HasForeignKey(mpr => mpr.MealPlanId);
        }
    }
}
