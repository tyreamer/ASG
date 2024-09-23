using System;

namespace ASGShared.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public string CuisineType { get; set; }
        public int Calories { get; set; }
        public string PrepTime { get; set; }
    }
}
