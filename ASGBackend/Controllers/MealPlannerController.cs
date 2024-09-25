using ASG.Services;
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
        public async Task<ActionResult<List<MealPlanRecipe>>> GetWeeklyPlan([FromQuery] string email)
        {
            try
            {
                var weeklyPlan = await _mealPlanService.GetWeeklyPlan(email);
                return Ok(weeklyPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly plan");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("regenerate")]
        public async Task<IActionResult> RegenerateRecipe([FromBody] RegenerateRecipeRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.UserPreferences))
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var newRecipe = await _mealPlanService.RegenerateRecipe(request.Email, request.UserPreferences, request.DayOfWeek, request.MealType);
                return Ok(newRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating recipe");
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

    public class RegenerateRecipeRequest
    {
        public string Email { get; set; }
        public string UserPreferences { get; set; }
        public int DayOfWeek { get; set; }
        public string MealType { get; set; }
    }
}