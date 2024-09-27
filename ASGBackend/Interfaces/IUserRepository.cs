using ASGShared.Models;

namespace ASGBackend.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserWithPreferences(Guid userId);
        Task AddRecipeRating(Guid userId, int recipeId, bool isLiked);
    }
}

