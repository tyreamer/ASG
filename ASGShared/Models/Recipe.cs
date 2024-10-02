using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASGShared.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string[] Ingredients { get; set; } = [];
        public string[] Instructions { get; set; } = [];
        public string CuisineType { get; set; }
        public string Calories { get; set; }
        public string PrepTime { get; set; }
        public string CookingTime { get; set; }
        public string TotalTime { get; set; }
    }
}
