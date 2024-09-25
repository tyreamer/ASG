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

        public string CreateRecipePrompt(UserPreferences userPreferences, List<string> previousRecipeTitles)
        {
            var userPreferencesString = _userPreferencesFormatter.FormatUserPreferences(userPreferences);


            var prompt = $"You are a recipe assistant. Based on the following preferences, generate a unique and varied recipe in JSON format:";
            prompt += $" Preferences: {userPreferencesString}.\n";
            prompt += $"The response must be in valid JSON format with the following fields: Name, Ingredients (array of strings), Instructions (array of strings), CuisineType, Calories, PrepTime.";

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
}
