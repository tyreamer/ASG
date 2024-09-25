using ASGShared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASGBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingListController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateShoppingList([FromBody] List<Recipe> selectedMeals)
        {
            var shoppingList = new List<string>();

            foreach (var meal in selectedMeals)
            {
                shoppingList.AddRange(meal.Ingredients);
            }

            return Ok(shoppingList.Distinct());  // Ensure ingredients are not duplicated
        }
    }

}
