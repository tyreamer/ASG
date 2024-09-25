using ASGShared.Models;
using System.Linq;

namespace ASGBackend.Helpers
{
    public class UserPreferencesFormatter
    {
        public string FormatUserPreferences(UserPreferences userPreferences)
        {
            var dietaryRestrictions = userPreferences.DietaryRestrictions != null
                ? string.Join(", ", userPreferences.DietaryRestrictions.GetType().GetProperties()
                    .Where(p => (bool)p.GetValue(userPreferences.DietaryRestrictions))
                    .Select(p => p.Name).DefaultIfEmpty("None"))
                : "None";

            var allergies = userPreferences.Allergies != null && userPreferences.Allergies.Any()
                ? string.Join(", ", userPreferences.Allergies)
                : "None";

            var favoriteCuisines = userPreferences.FavoriteCuisines != null && userPreferences.FavoriteCuisines.Any()
                ? string.Join(", ", userPreferences.FavoriteCuisines)
                : "None";

            var dislikedFoods = userPreferences.DislikedFoods != null && userPreferences.DislikedFoods.Any()
                ? string.Join(", ", userPreferences.DislikedFoods)
                : "None";

            var nutritionalGoals = userPreferences.NutritionalGoals != null
                ? string.Join(", ", userPreferences.NutritionalGoals.GetType().GetProperties()
                    .Where(p => (bool)p.GetValue(userPreferences.NutritionalGoals))
                    .Select(p => p.Name).DefaultIfEmpty("None"))
                : "None";

            var calorieTarget = userPreferences.CalorieTarget > 0
                ? userPreferences.CalorieTarget.ToString()
                : "Not specified";

            return $"Total Meals Per Week: {userPreferences.TotalMealsPerWeek}, " +
                   $"Dietary Restrictions: {dietaryRestrictions}, " +
                   $"Allergies: {allergies}, " +
                   $"Favorite Cuisines: {favoriteCuisines}, " +
                   $"Disliked Foods: {dislikedFoods}, " +
                   $"Nutritional Goals: {nutritionalGoals}, " +
                   $"Calorie Target: {calorieTarget}";
        }
    }
}
