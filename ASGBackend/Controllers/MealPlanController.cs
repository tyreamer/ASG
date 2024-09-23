using ASG.Services;
using ASGBackend.Data;
using ASGBackend.Interfaces;
using ASGBackend.Services;
using ASGShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealPlannerController : ControllerBase
    {
        private readonly AIAgentService _aiAgentService;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;
        private readonly MealPlanService _mealPlanService;
        private readonly ILogger<MealPlannerController> _logger;
        private readonly UserService _userService;

        public MealPlannerController(
            AIAgentService aiAgentService,
            IRecipeRepository recipeRepository,
            IUserRepository userRepository,
            MealPlanService mealPlanService,
            ILogger<MealPlannerController> logger,
            UserService userService)
        {
            _aiAgentService = aiAgentService;
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _mealPlanService = mealPlanService;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("weekly")]
        public ActionResult<List<Recipe>> GetWeeklyPlan()
        {
            try
            {
                var weeklyPlan = _mealPlanService.GetWeeklyPlan();
                return Ok(weeklyPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly plan");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("regenerate")]
        public IActionResult RegeneratePlan()
        {
            try
            {
                _mealPlanService.RegeneratePlan();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating plan");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("like")]
        public IActionResult LikeRecipe([FromBody] Recipe recipe)
        {
            try
            {
                _mealPlanService.LikeRecipe(recipe);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking recipe");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("dislike")]
        public IActionResult DislikeRecipe([FromBody] Recipe recipe)
        {
            try
            {
                _mealPlanService.DislikeRecipe(recipe);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disliking recipe");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, updatedUser);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
