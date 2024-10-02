using ASGShared.Models;

namespace ASGBackend.Helpers
{
    public class PromptCreator
    {
        private readonly UserPreferencesFormatter _userPreferencesFormatter;

        public PromptCreator(UserPreferencesFormatter userPreferencesFormatter)
        {
            _userPreferencesFormatter = userPreferencesFormatter;
        }

        public string CreateRecipePrompt(User user, List<string> previousRecipeTitles)
        {
            var userPreferencesString = _userPreferencesFormatter.FormatUserPreferences(user.Preferences);

            //todo add cooking skill level and household size
            var prompt = $"You are a recipe assistant. Based on the following, generate a delicious, unique, and varied recipe in JSON format:";
            prompt += $" Preferences: {userPreferencesString}.\n";
            prompt += $" Cooking Skill level (1-10 where 10 is expert): {user.CookingSkillLevel}.\n";
            prompt += $" Household size: {user.HouseholdSize}.\n";
            prompt += $" Budget per meal constraint: {user.BudgetPerMeal}.\n";
            prompt += $" Time constraint (total per recipe): {user.TotalTimeConstraintInMinutes}.\n";

            prompt += $"The response must be in valid JSON format with the following fields: Name, Ingredients (array of strings), Instructions (array of strings), CuisineType, Calories, PrepTime, CookingTime, TotalTime. (times should just be the minutes integer value)";
            prompt += $"Don't use abbreviations for ingredient units.";

            // Emphasize variety and uniqueness if previous recipes exist
            if (previousRecipeTitles?.Count > 0)
            {
                prompt += "\nIMPORTANT: The new recipe must be different from the following previous recipes. Focus on variety in ingredients, cooking methods, or cuisine types.";
                prompt += " Ensure there is no overlap with these recipe titles: " + string.Join(", ", previousRecipeTitles) + ".\n";
                prompt += "You may use a random factor of " + new Random().Next(1, 1000) + " to introduce some variability in the output.";
            }

            return prompt;
        }
    }

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
