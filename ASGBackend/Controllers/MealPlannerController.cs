using ASG.Services;
using ASGBackend.Interfaces;
using ASGBackend.Services;
using ASGShared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASGBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealPlannerController : ControllerBase
    {
        private readonly AIAgentService _aiAgentService;
        private readonly IRecipeRepository _recipeRepository;
        private readonly MealPlanService _mealPlanService;
        private readonly ILogger<MealPlannerController> _logger;

        public MealPlannerController(
            AIAgentService aiAgentService,
            IRecipeRepository recipeRepository,
            MealPlanService mealPlanService,
            ILogger<MealPlannerController> logger)
        {
            _aiAgentService = aiAgentService;
            _recipeRepository = recipeRepository;
            _mealPlanService = mealPlanService;
            _logger = logger;
        }

        [HttpGet("weekly")]
        public ActionResult<List<Recipe>> GetWeeklyPlan([FromQuery] string email)
        {
            try
            {
                var weeklyPlan = _mealPlanService.GetWeeklyPlan(email);
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
    }
}
