using System;
using System.Collections.Generic;

namespace ASGShared.Models
{
    public class MealPlan
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime WeekStartDate { get; set; }
        public List<MealPlanRecipe> Recipes { get; set; } = new List<MealPlanRecipe>();
    }
}
