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

        public async Task<MealPlan> GetWeeklyPlanAsync(Guid userId, DateTime? weekStartDate = null)
        {
            var request = $"api/mealplanner/weekly?userId={userId}";

            if (weekStartDate != null) {
                request += "&weekStartDate=" + weekStartDate;
            }

            var response = await _httpClient.GetFromJsonAsync<MealPlan>(request);

            return response ?? new MealPlan();
        }

        public async Task ReplaceRecipe(User user, int recipeId)
        {
            await _httpClient.PostAsJsonAsync($"api/mealplanner/{user.Id}/recipes/{recipeId}/replace", user);
        }

        public async Task DislikeRecipeAsync(Recipe recipe)
        {
            await _httpClient.PostAsJsonAsync("api/mealplanner/dislike", recipe);
        }

        public async Task RegenerateMealPlanAsync(User user, DateTime? weekStartDate = null)
        {
            await _httpClient.PostAsJsonAsync("api/mealplanner/regenerate/mealplan", user);
        }

        public async Task LikeRecipeAsync(Recipe recipe)
        {
            await _httpClient.PostAsJsonAsync("api/mealplanner/like", recipe);
        }
    }
}
