using ASGShared.Models;

namespace ASGBackend.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserWithPreferences(string userId);
        Task AddRecipeRating(string userId, int recipeId, bool isLiked);
    }
}

