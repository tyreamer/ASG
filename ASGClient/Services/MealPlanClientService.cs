using System.Net.Http.Json;
using System.Text.Json;
using ASGShared.Models;

namespace ASG.Services
{
    public class MealPlanClientService
    {
        private readonly HttpClient _httpClient;

        public MealPlanClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Recipe>> GetWeeklyPlanAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/mealplanner/weekly");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Recipe>>();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return new List<Recipe>();
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine($"The content type is not supported: {e.Message}");
                return new List<Recipe>();
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Invalid JSON: {e.Message}");
                return new List<Recipe>();
            }
        }

        public async Task RegeneratePlanAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/mealplanner/regenerate", null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        public async Task LikeRecipeAsync(Recipe recipe)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/mealplanner/like", recipe);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        public async Task DislikeRecipeAsync(Recipe recipe)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/mealplanner/dislike", recipe);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}

