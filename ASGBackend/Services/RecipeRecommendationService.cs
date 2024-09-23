using ASGBackend.Agents;
using ASGShared.Models;

namespace ASGBackend.Services
{
    public class RecipeRecommendationService
    {
        private readonly OpenAIService _openAIService;
        private readonly UserClusteringAgent _userClusteringAgent;
        private readonly Dictionary<int, string> _clusterRecipeCache;

        public RecipeRecommendationService(OpenAIService openAIService, UserClusteringAgent userClusteringAgent)
        {
            _openAIService = openAIService;
            _userClusteringAgent = userClusteringAgent;
            _clusterRecipeCache = new Dictionary<int, string>();
        }

        public async Task<string> GetRecipeRecommendation(User user)
        {
            int userCluster = _userClusteringAgent.PredictCluster(user);

            if (_clusterRecipeCache.TryGetValue(userCluster, out string cachedRecommendation))
            {
                return cachedRecommendation;
            }

            string userPreferences = $"Dietary restrictions: {string.Join(", ", user.Preferences.DietaryRestrictions)}, " +
                                        $"Cuisine preferences: {string.Join(", ", user.Preferences.FavoriteCuisines)}, " +
                                        $"Budget Per Meal: {user.BudgetPerMeal} {user.BudgetPerMeal.Currency}, " +
                                        $"Household size: {user.HouseholdSize}, " +
                                        $"Cooking skill level: {user.CookingSkillLevel}";

            string recommendation = await _openAIService.GenerateRecipeRecommendation(userPreferences);

            _clusterRecipeCache[userCluster] = recommendation;

            return recommendation;
        }
    }
}
