using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASGShared.Models
{
    public class User
    {
        public bool IsAuthenticated { get; set; }

        [NotMapped]
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Budget BudgetPerMeal { get; set; } = new Budget();
        public int HouseholdSize { get; set; } = 1;
        public UserPreferences? Preferences { get; set; }

        private int _cookingSkillLevel = 1;
        public int CookingSkillLevel
        {
            get => _cookingSkillLevel;
            set
            {
                if (value < 1 || value > 10)
                {
                    throw new ArgumentOutOfRangeException(nameof(CookingSkillLevel), "CookingSkillLevel must be between 1 and 10.");
                }
                _cookingSkillLevel = value;
            }
        }
    }
}
