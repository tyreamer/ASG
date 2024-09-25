using ASGBackend.Services;
using ASGShared.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ASGBackend.Data;

namespace ASG.Services
{
    public class MealPlanService
    {
        private readonly ILogger<MealPlanService> _logger;
        private readonly OpenAIService _openAIService;
        private readonly ApplicationDbContext _dbContext;

        public MealPlanService(ILogger<MealPlanService> logger, OpenAIService openAIService, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _openAIService = openAIService;
            _dbContext = dbContext;
        }

        public async Task<List<MealPlanRecipe>> GetWeeklyPlan(string email)
        {
            try
            {
                var mealPlan = await _dbContext.MealPlans
                    .Include(mp => mp.Recipes)
                    .ThenInclude(mpr => mpr.RecipeId)
                    .FirstOrDefaultAsync(mp => mp.UserId == email);

                if (mealPlan != null)
                {
                    return mealPlan.Recipes;
                }

                // Return an empty list if no meal plan is found
                return new List<MealPlanRecipe>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly plan");
                throw;
            }
        }

        public async Task<MealPlanRecipe> RegenerateRecipe(string email, string userPreferences, int dayOfWeek, string mealType)
        {
            try
            {
                var newRecipe = await _openAIService.GenerateRecipeRecommendation(userPreferences);

                var mealPlan = await _dbContext.MealPlans
                    .Include(mp => mp.Recipes)
                    .FirstOrDefaultAsync(mp => mp.UserId == email);

                if (mealPlan == null)
                {
                    mealPlan = new MealPlan { UserId = email, Recipes = new List<MealPlanRecipe>() };
                    _dbContext.MealPlans.Add(mealPlan);
                }

                var mealPlanRecipe = new MealPlanRecipe
                {
                    RecipeId = newRecipe.Id,
                    DayOfWeek = dayOfWeek,
                    MealType = mealType
                };

                mealPlan.Recipes.Add(mealPlanRecipe);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"Generated new recipe for {mealType} on day {dayOfWeek}: {newRecipe.Name}");
                return mealPlanRecipe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating plan");
                throw;
            }
        }

        public void LikeRecipe(MealPlanRecipe recipe)
        {
            try
            {
                // Implement like logic
                Console.WriteLine($"Liked recipe: {recipe.RecipeId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking recipe");
                throw;
            }
        }

        public void DislikeRecipe(MealPlanRecipe recipe)
        {
            try
            {
                // Implement dislike logic
                Console.WriteLine($"Disliked recipe: {recipe.RecipeId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disliking recipe");
                throw;
            }
        }
    }
}

