using ASGShared.Models;
using ASGBackend.Interfaces;
using ASGBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace ASGBackend.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRecipe(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
        }
    }
}
