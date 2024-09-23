using System.Collections.Generic;
using System.Threading.Tasks;
using ASGShared.Models;
using ASGBackend.Interfaces;

namespace ASGBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = [];
        private readonly List<(string UserId, int RecipeId, bool IsLiked)> _ratings = [];

        public Task<User> GetUserWithPreferences(string userId)
        {
            var user = _users.Find(u => u.Id == userId);
            return Task.FromResult(user);
        }

        public Task AddRecipeRating(string userId, int recipeId, bool isLiked)
        {
            _ratings.Add((userId, recipeId, isLiked));
            return Task.CompletedTask;
        }
    }
}

