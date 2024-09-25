using System;
using System.Text.Json.Serialization;

namespace ASGShared.Models
{
    public class MealPlanRecipe
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int MealPlanId { get; set; }
        public int DayOfWeek { get; set; }

        public Recipe Recipe { get; set; } = null!; 
        
        [JsonIgnore]
        public MealPlan MealPlan { get; set; } = null!;
    }
}
