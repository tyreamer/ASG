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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation($"Fetching user with email: {email}");
                var user = await _context.Users
                    .Include(u => u.DietaryRestrictionsUsers)
                    .ThenInclude(dru => dru.DietaryRestrictions)
                    .FirstOrDefaultAsync(u => u.Email == email);
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

        public async Task<User> UpdateUserAsync(string id, User updatedUser)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return null;
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                // Update other properties as needed

                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
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

        public async Task<bool> ValidateFullyRegistered(string id)
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
    }
}
