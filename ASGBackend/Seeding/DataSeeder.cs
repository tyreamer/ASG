using ASGBackend.Data;
using ASGShared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DataSeeder
{
    public static void SeedInitialData(ApplicationDbContext context)
    {
        if (!context.Recipes.Any())
        {
            var recipes = new List<Recipe>
            {
                new Recipe { Name = "Vegetarian Lasagna", Calories = "450", CookingTime = "45 min", CuisineType = "Italian", Ingredients = new[] { "cheese", "pasta", "pasta sauce" }, Instructions = new[] { "1. preheat oven", "2. start cooking", "3. eat" }, PrepTime = "15 min", TotalTime = "60 min" },
                new Recipe { Name = "Grilled Salmon with Asparagus", Calories = "380", CookingTime = "30 min", CuisineType = "American", Ingredients = new[] { "salmon", "asparagus", "lemon", "rice" }, Instructions = new[] { "1. preheat oven", "2. start cooking", "3. eat" }, PrepTime = "10 min", TotalTime = "40 min" },
                new Recipe { Name = "Chicken Stir Fry", Calories = "420", CookingTime = "25 min", CuisineType = "Asian", Ingredients = new[] { "chicken", "vegetables", "soy sauce" }, Instructions = new[] { "1. preheat oven", "2. start cooking", "3. eat" }, PrepTime = "10 min", TotalTime = "35 min" },
                new Recipe { Name = "Quinoa Salad with Roasted Vegetables", Calories = "350", CookingTime = "20 min", CuisineType = "Mediterranean", Ingredients = new[] { "quinoa", "vegetables", "olive oil" }, Instructions = new[] { "1. preheat oven", "2. start cooking", "3. eat" }, PrepTime = "10 min", TotalTime = "30 min" },
                new Recipe { Name = "Homemade Pizza", Calories = "500", CookingTime = "40 min", CuisineType = "Italian", Ingredients = new[] { "pizza dough", "marinara", "pepperoni", "cheese" }, Instructions = new[] { "1. preheat oven", "2. start cooking", "3. eat" }, PrepTime = "20 min", TotalTime = "60 min" },
                new Recipe { Name = "Beef Tacos", Calories = "460", CookingTime = "35 min", CuisineType = "Mexican", Ingredients = new[] { "taco seasoning", "beef", "tortillas" }, Instructions = new[] { "1. preheat oven", "2. start cooking", "3. eat" }, PrepTime = "15 min", TotalTime = "50 min" },
                new Recipe { Name = "Lentil Soup", Calories = "300", CookingTime = "50 min", CuisineType = "Middle Eastern", Ingredients = new[] { "broth", "lentils", "salt" }, Instructions = new[] { "1. preheat oven", "2. start cooking", "3. eat" }, PrepTime = "10 min", TotalTime = "60 min" },
            };

            context.Recipes.AddRange(recipes);
            context.SaveChanges();
        }

        if (!context.MealPlans.Any())
        {
            var mealPlans = new List<MealPlan>
            {
                new MealPlan
                {
                    UserId = new Guid(),
                    Recipes = new List<MealPlanRecipe>
                    {
                        new MealPlanRecipe { RecipeId = 1},
                        new MealPlanRecipe { RecipeId = 2},
                        new MealPlanRecipe { RecipeId = 3}
                    }
                }
            };

            context.MealPlans.AddRange(mealPlans);
            context.SaveChanges();
        }
    }
}
