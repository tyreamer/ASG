using ASGBackend.Services;
using ASGBackend.Helpers;
using ASGShared.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ASGBackend.Interfaces;
using ASGBackend.Repositories;
using ASGShared.Helpers;

namespace ASG.Services
{
    public class MealPlanService
    {
        private readonly ILogger<MealPlanService> _logger;
        private readonly GeminiService _geminiService;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMealPlanRepository _mealPlanRepository;

        public MealPlanService(ILogger<MealPlanService> logger, GeminiService geminiService, IRecipeRepository recipeRepository, IMealPlanRepository mealPlanRepository)
        {
            _logger = logger;
            _geminiService = geminiService;
            _recipeRepository = recipeRepository;
            _mealPlanRepository = mealPlanRepository;
        }

        public async Task<MealPlan> GetWeeklyPlan(Guid userId, DateTime weekStarted)
        {
            try
            {
                var mealPlan = await _mealPlanRepository.GetMealPlan(userId, weekStarted);

                if (mealPlan != null)
                {
                    return mealPlan;
                }

                // Return an empty list if no meal plan is found
                return new MealPlan();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly plan");
                throw;
            }
        }

        public async Task<MealPlanRecipe> ReplaceRecipeAsync(User user, int recipeId, DateTime? weekStarted = null)
        {
            try
            {
                var weekStartDate = weekStarted ?? DateTime.Now;

                // Get the user's meal plan
                var mealPlan = await _mealPlanRepository.GetMealPlanByUserIdAsync(user.Id, weekStartDate);
                if (mealPlan == null)
                {
                    throw new Exception("Meal plan not found.");
                }

                var otherRecipes = new List<string>();
                foreach (var recipe in mealPlan.Recipes)
                {
                    if (recipe.RecipeId != recipeId && !string.IsNullOrEmpty(recipe.Recipe?.Name))
                    {
                        otherRecipes.Add(recipe.Recipe.Name);
                    }
                }

                // Generate a new recipe based on user preferences
                var newRecipe = await RegenerateRecipe(user, otherRecipes);

                // Find and remove the old recipe from the meal plan
                var oldRecipe = mealPlan.Recipes.FirstOrDefault(r => r.RecipeId == recipeId);
                if (oldRecipe != null)
                {
                    mealPlan.Recipes.Remove(oldRecipe);
                }

                // Create a new MealPlanRecipe and assign the MealPlan and MealPlanId
                var newMealPlanRecipe = new MealPlanRecipe
                {
                    RecipeId = newRecipe.Id,
                    MealPlan = mealPlan,
                    MealPlanId = mealPlan.Id,
                    DayOfWeek = oldRecipe?.DayOfWeek ?? 0, // Assuming you want to keep the same day of the week
                    Recipe = newRecipe
                }; ;

                mealPlan.Recipes.Add(newMealPlanRecipe);

                await _mealPlanRepository.UpdateMealPlanAsync(mealPlan);

                _logger.LogInformation($"Replaced recipe {recipeId} with new recipe {newRecipe.Id} for user {user.Id}");

                return newMealPlanRecipe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error replacing recipe");
                throw;
            }
        }

        public async Task<Recipe> RegenerateRecipe(User user, List<string>? otherRecipesInMealPlan = null)
        {
            try
            {
                var userPreferencesFormatter = new UserPreferencesFormatter();
                var promptCreator = new PromptCreator(userPreferencesFormatter);
                var prompt = promptCreator.CreateRecipePrompt(user, otherRecipesInMealPlan ?? new List<string>());

                var newRecipeContent = await _geminiService.GenerateRecipe(prompt);

                // Check if the content is JSON or plain text
                if (!IsJson(newRecipeContent))
                {
                    newRecipeContent = ConvertPlainTextToJson(newRecipeContent);
                }

                // Cleanse the JSON content
                var cleansedRecipeContent = CleanseRecipeContent(newRecipeContent);

                // Deserialize the new recipe content
                var newRecipe = JsonSerializer.Deserialize<Recipe>(cleansedRecipeContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (newRecipe == null)
                {
                    _logger.LogError("Generated recipe is null.");
                    throw new Exception("Generated recipe is null.");
                }

                // Save the new recipe to the database to get the auto-generated ID
                await _recipeRepository.AddRecipe(newRecipe);

                _logger.LogInformation($"Generated new recipe: {newRecipe.Name}");
                return newRecipe;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx.Message, jsonEx.InnerException);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                throw;
            }
        }

        private bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}") || input.StartsWith("[") && input.EndsWith("]");
        }

        private string ConvertPlainTextToJson(string plainText)
        {
            // Implement a method to convert plain text to JSON format
            // This is a simplified example and may need to be adjusted based on the actual plain text format
            var lines = plainText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var jsonObject = new Dictionary<string, object>
            {
                { "Title", lines[0] },
                { "Description", lines[1] },
                { "Ingredients", lines.Skip(3).TakeWhile(line => !line.StartsWith("**Instructions:**")).ToList() },
                { "Instructions", lines.SkipWhile(line => !line.StartsWith("**Instructions:**")).Skip(1).TakeWhile(line => !line.StartsWith("**Tips:**")).ToList() },
                { "Tips", lines.SkipWhile(line => !line.StartsWith("**Tips:**")).Skip(1).ToList() }
            };

            return JsonSerializer.Serialize(jsonObject);
        }

        public async Task<MealPlan> RegenerateMealPlanAsync(User user, DateTime weekStarted)
        {
            var mealPlan = await _mealPlanRepository.GetMealPlan(user.Id, weekStarted);

            if (mealPlan != null && mealPlan.Recipes?.Count > 0)
            {
                mealPlan.Recipes.Clear();
                await _mealPlanRepository.SaveChangesAsync();
            }
            else
            {
                // No meal plan for this week yet, create a new one
                mealPlan = new MealPlan
                {
                    UserId = user.Id,
                    WeekStartDate = weekStarted
                };

                // Add the new meal plan to the repository and save to get the generated ID
                await _mealPlanRepository.AddMealPlanAsync(mealPlan);
                await _mealPlanRepository.SaveChangesAsync();
            }

            // Generate new recipes for the meal plan
            var newRecipes = await GenerateWeeklyRecipes(user);

            // Set MealPlan and MealPlanId for each new recipe
            foreach (var recipe in newRecipes)
            {
                recipe.MealPlan = mealPlan;
                recipe.MealPlanId = mealPlan.Id;
            }

            mealPlan.Recipes = newRecipes;
            await _mealPlanRepository.SaveChangesAsync();

            return mealPlan;
        }

        private async Task<List<MealPlanRecipe>> GenerateWeeklyRecipes(User user)
        {
            var newRecipes = new List<MealPlanRecipe>();
            int numberOfRecipes = user.Preferences.TotalMealsPerWeek;
            var currentRecipeTitles = new List<string>();

            for (int i = 0; i < numberOfRecipes; i++)
            {
                var newRecipe = await RegenerateRecipe(user, currentRecipeTitles);

                // Add the new recipe title to the list to avoid repetition
                if (!string.IsNullOrEmpty(newRecipe.Name))
                {
                    currentRecipeTitles.Add(newRecipe.Name);
                }

                var mealPlanRecipe = new MealPlanRecipe
                {
                    RecipeId = newRecipe.Id,
                    Recipe = newRecipe,
                    DayOfWeek = i // Assign each recipe to a day of the week
                };

                newRecipes.Add(mealPlanRecipe);
            }

            return newRecipes;
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

        private string CleanseRecipeContent(string recipeContent)
        {
            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            // Parse the JSON content with the specified options
            var jsonDocument = JsonDocument.Parse(recipeContent, options);
            var root = jsonDocument.RootElement;

            // Create a mutable dictionary to hold the cleansed content
            var cleansedContent = new Dictionary<string, object>();

            foreach (var property in root.EnumerateObject())
            {
                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.Number:
                        cleansedContent[property.Name] = property.Value.GetInt32().ToString();
                        break;
                    case JsonValueKind.String:
                        cleansedContent[property.Name] = property.Value.GetString() ?? string.Empty;
                        break;
                    case JsonValueKind.Array:
                        cleansedContent[property.Name] = property.Value.EnumerateArray().Select(element => element.ToString()).ToArray();
                        break;
                    case JsonValueKind.Object:
                        cleansedContent[property.Name] = property.Value.ToString();
                        break;
                    default:
                        cleansedContent[property.Name] = property.Value.ToString();
                        break;
                }
            }

            // Serialize the cleansed content back to JSON
            return JsonSerializer.Serialize(cleansedContent);
        }
    }
}
