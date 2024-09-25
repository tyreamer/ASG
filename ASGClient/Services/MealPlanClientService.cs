using ASGShared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ASG.Services
{
    public class MealPlanClientService
    {
        private readonly HttpClient _httpClient;

        public MealPlanClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Recipe>> GetWeeklyPlanAsync(string email)
        {
            var response = await _httpClient.GetAsync($"api/mealplanner/weekly?email={email}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Recipe>>() ?? new List<Recipe>();
        }

        public async Task RegeneratePlanAsync()
        {
            var response = await _httpClient.PostAsync("api/mealplanner/regenerate", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task LikeRecipeAsync(Recipe recipe)
        {
            var response = await _httpClient.PostAsJsonAsync("api/mealplanner/like", recipe);
            response.EnsureSuccessStatusCode();
        }

        public async Task DislikeRecipeAsync(Recipe recipe)
        {
            var response = await _httpClient.PostAsJsonAsync("api/mealplanner/dislike", recipe);
            response.EnsureSuccessStatusCode();
        }
    }
}
