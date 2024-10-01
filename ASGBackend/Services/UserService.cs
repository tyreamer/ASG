using ASGBackend.Data;
using ASGShared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGBackend.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation($"Fetching user with id: {userId}");

                var user = await _context.Users
                    .Include(u => u.Preferences)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning($"User {userId} not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user");
                throw;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation($"Fetching user with email: {email}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogWarning($"User with email {email} not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user");
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(Guid id, User updatedUser)
        {
            var user = await _context.Users.Include(u => u.Preferences).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            user.Email = updatedUser.Email;
            user.Name = updatedUser.Name;
            user.DisplayName = updatedUser.DisplayName;
            user.BudgetPerMeal = updatedUser.BudgetPerMeal;
            user.HouseholdSize = updatedUser.HouseholdSize;
            user.CookingSkillLevel = updatedUser.CookingSkillLevel;

            if (user.Preferences != null && updatedUser.Preferences != null)
            {
                user.Preferences.TotalMealsPerWeek = updatedUser.Preferences.TotalMealsPerWeek;
                user.Preferences.DietaryRestrictions = updatedUser.Preferences.DietaryRestrictions;
                user.Preferences.Allergies = updatedUser.Preferences.Allergies;
                user.Preferences.FavoriteCuisines = updatedUser.Preferences.FavoriteCuisines;
                user.Preferences.DislikedFoods = updatedUser.Preferences.DislikedFoods;
                user.Preferences.NutritionalGoals = updatedUser.Preferences.NutritionalGoals;
                user.Preferences.CalorieTarget = updatedUser.Preferences.CalorieTarget;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                throw;
            }
        }

        public async Task<bool> ValidateFullyRegistered(Guid id)
        {
            try
            {
                _logger.LogInformation($"Validating if user with id: {id} is fully registered");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning($"User with id {id} not found");
                    return false;
                }

                // Check if all required fields are filled out
                if (string.IsNullOrWhiteSpace(user.Name) ||
                    string.IsNullOrWhiteSpace(user.Email) ||
                    string.IsNullOrWhiteSpace(user.DisplayName) ||
                    user.HouseholdSize <= 0 ||
                    user.CookingSkillLevel < 1 || user.CookingSkillLevel > 10)
                {
                    _logger.LogWarning($"User with id {id} is not fully registered");
                    return false;
                }

                _logger.LogInformation($"User with id {id} is fully registered");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating if user is fully registered");
                throw;
            }
        }

        public async Task<UserPreferences?> GetUserPreferencesAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Preferences)
                                               .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.Preferences == null)
                {
                    _logger.LogWarning($"User or preferences for user {userId} not found");
                    return null;
                }

                return user.Preferences;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user preferences");
                throw;
            }
        }

        public async Task<UserPreferences> UpdateUserPreferencesAsync(Guid id, UserPreferences updatedPreferences)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Preferences)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null || updatedPreferences == null)
                {
                    //we shouldn't be here...but just in case
                    return null;
                }

                if (user.Preferences == null)
                {
                    //new sign-up
                    user.Preferences = new UserPreferences();
                }

                user.Preferences.UserId = user.Id;
                user.Preferences.TotalMealsPerWeek = updatedPreferences.TotalMealsPerWeek;
                user.Preferences.DietaryRestrictions = updatedPreferences.DietaryRestrictions;
                user.Preferences.Allergies = updatedPreferences.Allergies;
                user.Preferences.FavoriteCuisines = updatedPreferences.FavoriteCuisines;
                user.Preferences.DislikedFoods = updatedPreferences.DislikedFoods;
                user.Preferences.NutritionalGoals = updatedPreferences.NutritionalGoals;
                user.Preferences.CalorieTarget = updatedPreferences.CalorieTarget;

                await _context.SaveChangesAsync();
                return user.Preferences;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user preferences");
                throw;
            }
        }
    }
}
