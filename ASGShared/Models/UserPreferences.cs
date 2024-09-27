using System.ComponentModel.DataAnnotations.Schema;

namespace ASGShared.Models
{
    public class UserPreferences
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int TotalMealsPerWeek { get; set; } = 3; // default to 3
        public DietaryRestrictions DietaryRestrictions { get; set; } = new DietaryRestrictions();
        public List<string> Allergies { get; set; } = [];
        public List<string> FavoriteCuisines { get; set; } = [];
        public List<string> DislikedFoods { get; set; } = [];
        public NutritionalGoals NutritionalGoals { get; set; } = new NutritionalGoals();
        public int CalorieTarget { get; set; } = 2000;
    }

    public class NutritionalGoals
    {
        public bool HighProtein { get; set; }
        public bool LowCarb { get; set; }
        public bool LowFat { get; set; }
    }

    public class DietaryRestrictions
    {
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsPescatarian { get; set; }
    }
}
