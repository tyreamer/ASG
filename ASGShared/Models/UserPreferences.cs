namespace ASGShared.Models
{
    public class UserPreferences
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DietaryRestrictions DietaryRestrictions { get; set; } = new DietaryRestrictions();
        public List<string> Allergies { get; set; } = [];
        public List<string> FavoriteCuisines { get; set; } = [];
        public List<string> DislikedFoods { get; set; } = [];
        public NutritionalGoals NutritionalGoals { get; set; } = new NutritionalGoals();
    }
}
