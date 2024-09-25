using System;
using System.Collections.Generic;

namespace ASGShared.Models
{
    public class MealPlan
    {
        public int Id { get; set; } // Primary key
        public string UserId { get; set; } // User identifier
        public DateTime WeekStartDate { get; set; } // Start date of the meal plan week
        public List<MealPlanRecipe> Recipes { get; set; } = new List<MealPlanRecipe>(); // List of MealPlanRecipe objects

        // Constructor to initialize the list
        public MealPlan()
        {
            Recipes = new List<MealPlanRecipe>();
        }
    }
}

