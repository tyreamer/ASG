using ASG.Services;
using ASGShared.Helpers;
using ASGBackend.Interfaces;
using ASGBackend.Services;
using ASGShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<MealPlan>> GetWeeklyPlan([FromQuery] Guid userId, [FromQuery] DateTime? weekStartDate = null)
        {
            try
            {
                if (weekStartDate == null) {
                    weekStartDate = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
                }

                var weeklyPlan = await _mealPlanService.GetWeeklyPlan(userId, weekStartDate.Value);
                return Ok(weeklyPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly plan");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{userId}/recipes/{recipeId}/replace")]
        public async Task<IActionResult> ReplaceRecipe([FromBody] User user, int recipeId) //TODO add week started 
        {
            if (user == null || user.Preferences == null || recipeId <= 0 || user.Id == Guid.NewGuid())
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var newRecipe = await _mealPlanService.ReplaceRecipeAsync(user, recipeId);
                return Ok(newRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error replacing recipe");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("recipes/generate")]
        public async Task<IActionResult> GenerateRecipe([FromBody] User user) //TODO add week started 
        {
            if (user == null || user.Preferences == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var newRecipe = await _mealPlanService.RegenerateRecipe(user);
                return Ok(newRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating recipe");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("regenerate/mealplan")]
        public async Task<IActionResult> RegenerateMealPlan([FromBody] User user, DateTime? weekStartDate = null)
        {
            if (weekStartDate == null)
            {
                weekStartDate = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
            }

            if (user == null || user.Id == Guid.NewGuid() || user.Preferences == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                //TODO: add weekstarted
                var newMealPlan = await _mealPlanService.RegenerateMealPlanAsync(user, weekStartDate.Value);
                return Ok(newMealPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating meal plan");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("like")]
        public IActionResult LikeRecipe([FromBody] MealPlanRecipe recipe)
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
        public IActionResult DislikeRecipe([FromBody] MealPlanRecipe recipe)
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