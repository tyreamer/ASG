using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASGShared.Models;

namespace ASGBackend.Interfaces
{
    public interface IRecipeRepository
    {
        Task AddRecipe(Recipe recipe);
        Task<MealPlan> GetMealPlan(string userId, DateTime weekStartDate);
        Task AddMealPlan(MealPlan mealPlan);
    }
}