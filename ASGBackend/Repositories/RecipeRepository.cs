using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASGShared.Models;
using ASGBackend.Interfaces;

namespace ASGBackend.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly List<Recipe> _recipes = [];
        private readonly List<MealPlan> _mealPlans = [];

        public Task AddRecipe(Recipe recipe)
        {
            recipe.RecipeId = _recipes.Count + 1;
            _recipes.Add(recipe);
            return Task.CompletedTask;
        }

        public Task<MealPlan> GetMealPlan(string userId, DateTime weekStartDate)
        {
            var mealPlan = _mealPlans.Find(mp => mp.UserId == userId && mp.WeekStartDate == weekStartDate);
            return Task.FromResult(mealPlan);
        }

        public Task AddMealPlan(MealPlan mealPlan)
        {
            mealPlan.MealPlanId = _mealPlans.Count + 1;
            _mealPlans.Add(mealPlan);
            return Task.CompletedTask;
        }
    }
}

