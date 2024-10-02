using ASGBackend.Agents;
using ASGShared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASGBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAgentController : ControllerBase
    {
        private readonly AIAgentService _aiAgentService;
        private readonly UserClusteringAgent _userClusteringAgent;
        private readonly RecipeClusteringAgent _recipeClusteringAgent;

        public AIAgentController(AIAgentService aiAgentService, UserClusteringAgent userClusteringAgent, RecipeClusteringAgent recipeClusteringAgent)
        {
            _aiAgentService = aiAgentService;
            _userClusteringAgent = userClusteringAgent;
            _recipeClusteringAgent = recipeClusteringAgent;
        }

        [HttpPost("classify-recipe")]
        public IActionResult ClassifyRecipe([FromBody] Recipe recipe)
        {
            var classification = _aiAgentService.ClassifyRecipeAsync(recipe);
            return Ok(classification);
        }

        [HttpPost("train-clustering-model")]
        public IActionResult TrainClusteringModel()
        {
            _userClusteringAgent.TrainModelAsync();
            _recipeClusteringAgent.TrainModelAsync();
            return Ok();
        }

        [HttpPost("predict-user-cluster")]
        public IActionResult PredictCluster([FromBody] User user)
        {
            var userFeatures = new UserFeatures
            {
                HouseholdSize = user.HouseholdSize,
                TotalTimeConstraintInMinutes = user.TotalTimeConstraintInMinutes,
                CookingSkillLevel = user.CookingSkillLevel,
                TotalMealsPerWeek = user.Preferences?.TotalMealsPerWeek ?? 0,
                CalorieTarget = user.Preferences?.CalorieTarget ?? 0,
                IsVegetarian = user.Preferences?.DietaryRestrictions.IsVegetarian ?? false,
                IsVegan = user.Preferences?.DietaryRestrictions.IsVegan ?? false,
                IsGlutenFree = user.Preferences?.DietaryRestrictions.IsGlutenFree ?? false,
                IsPescatarian = user.Preferences?.DietaryRestrictions.IsPescatarian ?? false,
                HighProtein = user.Preferences?.NutritionalGoals.HighProtein ?? false,
                LowCarb = user.Preferences?.NutritionalGoals.LowCarb ?? false,
                LowFat = user.Preferences?.NutritionalGoals.LowFat ?? false
            };

            var clusterId = _userClusteringAgent.Predict(userFeatures);
            return Ok(clusterId);
        }

        [HttpPost("predict-recipe-cluster")]
        public IActionResult PredictRecipeCluster([FromBody] Recipe recipe)
        {
            var clusterId = _recipeClusteringAgent.PredictCluster(recipe);
            return Ok(clusterId);
        }
    }
}
