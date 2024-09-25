using System;

namespace ASGShared.Models
{
    public class MealPlanRecipe
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int MealPlanId { get; set; }
        public int DayOfWeek { get; set; }
        public string MealType { get; set; }

        public Recipe Recipe { get; set; } = null!;
        public MealPlan MealPlan { get; set; } = null!;
    }
}
