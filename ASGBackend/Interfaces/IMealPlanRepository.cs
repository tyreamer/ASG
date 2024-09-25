using ASGShared.Models;
using System.Threading.Tasks;

namespace ASGBackend.Interfaces
{
    public interface IMealPlanRepository
    {
        Task<MealPlan?> GetMealPlanByUserIdAsync(string userId);
        Task<MealPlan?> GetMealPlan(string userId, DateTime weekStartDate);
        Task<MealPlan> AddMealPlanAsync(MealPlan mealPlan);
        Task UpdateMealPlanAsync(MealPlan mealPlan);
        Task SaveChangesAsync();
    }
}
