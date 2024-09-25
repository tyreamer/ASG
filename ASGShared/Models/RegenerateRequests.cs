namespace ASGShared.Models
{
    public class GenerateRecipeRequest
    {
        public UserPreferences UserPreferences { get; set; }
    }

    public class RegenerateMealPlanRequest
    {
        public string Email { get; set; }
        public UserPreferences UserPreferences { get; set; }
    }
}
