using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ASGShared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ASG.Services
{
    public class MealPlanClientService
    {
        private readonly HttpClient _httpClient;

        public MealPlanClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MealPlan> GetWeeklyPlanAsync(Guid userId, DateTime weekStartDate)
        {
            var request = $"api/mealplanner/weekly?userId={userId}";

            if (weekStartDate != null) {
                request += "&weekStartDate=" + weekStartDate.ToString("yyyy-MM-dd");
            }

            var response = await _httpClient.GetFromJsonAsync<MealPlan>(request);

            return response ?? new MealPlan();
        }

        public async Task ReplaceRecipe(User user, int recipeId, DateTime weekStarted)
        {
            var formattedDate = weekStarted.ToString("yyyy-MM-dd");
            var url = $"api/mealplanner/{user.Id}/mealPlan/{formattedDate}/recipes/{recipeId}/replace";

            var response = await _httpClient.PostAsJsonAsync(url, user);

            // Log the response status
            Console.WriteLine($"ReplaceRecipe Response Status: {response.StatusCode}");

            response.EnsureSuccessStatusCode();
        }


        public async Task DislikeRecipeAsync(Recipe recipe)
        {
            await _httpClient.PostAsJsonAsync("api/mealplanner/dislike", recipe);
        }

        public async Task RegenerateMealPlanAsync(User user, DateTime weekStartDate)
        {
            var requestUri = $"api/mealplanner/regenerate/mealplan?weekStartDate={weekStartDate.ToString("o")}";
            var response = await _httpClient.PostAsJsonAsync(requestUri, user);

            if (!response.IsSuccessStatusCode)
            {
                // Handle error response
                throw new Exception("Failed to regenerate meal plan.");
            }
        }

        public async Task LikeRecipeAsync(Recipe recipe)
        {
            await _httpClient.PostAsJsonAsync("api/mealplanner/like", recipe);
        }
    }
}
