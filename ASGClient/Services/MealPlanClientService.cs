using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ASGShared.Models;
using System.Collections.Generic;
using System.Linq;

namespace ASG.Services
{
    public class MealPlanClientService
    {
        private readonly HttpClient _httpClient;

        public MealPlanClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MealPlan> GetWeeklyPlanAsync(Guid userId)
        {
            var response = await _httpClient.GetFromJsonAsync<MealPlan>($"api/mealplanner/weekly?userId={userId}");

            return response ?? new MealPlan();
        }

        public async Task ReplaceRecipe(Guid userId, int recipeId, UserPreferences userPreferences)
        {
            var request = new GenerateRecipeRequest { UserPreferences = userPreferences };
            await _httpClient.PostAsJsonAsync($"api/mealplanner/{userId}/recipes/{recipeId}/replace", request);
        }

        public async Task DislikeRecipeAsync(Recipe recipe)
        {
            await _httpClient.PostAsJsonAsync("api/mealplanner/dislike", recipe);
        }

        public async Task RegenerateMealPlanAsync(Guid userId, UserPreferences userPreferences)
        {
            var request = new RegenerateMealPlanRequest { UserId = userId, UserPreferences = userPreferences };
            await _httpClient.PostAsJsonAsync("api/mealplanner/regenerate/mealplan", request);
        }

        public async Task LikeRecipeAsync(Recipe recipe)
        {
            await _httpClient.PostAsJsonAsync("api/mealplanner/like", recipe);
        }
    }
}
