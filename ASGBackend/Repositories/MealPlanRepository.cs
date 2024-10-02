using ASGBackend.Data;
using ASGBackend.Interfaces;
using ASGShared.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASGBackend.Repositories
{
    public class MealPlanRepository : IMealPlanRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MealPlanRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MealPlan?> GetMealPlanByUserIdAsync(Guid userId, DateTime weekStartDate)
        {
            return await _dbContext.MealPlans
                .Include(mp => mp.Recipes)
                .ThenInclude(mpr => mpr.Recipe)
                .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.WeekStartDate.Date == weekStartDate.Date);
        }

        public async Task<MealPlan?> GetMealPlan(Guid userId, DateTime weekStartDate)
        {
            var result = _dbContext.MealPlans
                .Include(mp => mp.Recipes)
                .ThenInclude(mpr => mpr.Recipe);

            return await result.FirstOrDefaultAsync(mp => mp.UserId == userId && mp.WeekStartDate.Date == weekStartDate.Date);
        }

        public async Task<MealPlan> AddMealPlanAsync(MealPlan mealPlan)
        {
            _dbContext.MealPlans.Add(mealPlan);
            await _dbContext.SaveChangesAsync();
            return mealPlan;
        }

        public async Task UpdateMealPlanAsync(MealPlan mealPlan)
        {
            _dbContext.MealPlans.Update(mealPlan);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
