using Microsoft.AspNetCore.Mvc;
using ASGBackend.Services;
using ASGBackend.Agents;
using ASGShared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASGBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAgentController : ControllerBase
    {
        private readonly AIAgentService _aiAgentService;
        private readonly UserClusteringAgent _userClusteringAgent;

        public AIAgentController(AIAgentService aiAgentService, UserClusteringAgent userClusteringAgent)
        {
            _aiAgentService = aiAgentService;
            _userClusteringAgent = userClusteringAgent;
        }

        [HttpPost("generate-recipe")]
        public async Task<IActionResult> GenerateRecipe([FromBody] UserPreferences preferences, [FromBody] Budget budget)
        {
            var recipe = await _aiAgentService.GenerateRecipe(preferences, budget);
            return Ok(recipe);
        }

        [HttpPost("classify-recipe")]
        public IActionResult ClassifyRecipe([FromBody] Recipe recipe)
        {
            var classification = _aiAgentService.ClassifyRecipe(recipe);
            return Ok(classification);
        }

        [HttpPost("train-clustering-model")]
        public IActionResult TrainClusteringModel([FromBody] IEnumerable<User> users)
        {
            _userClusteringAgent.TrainModel(users);
            return Ok("Model trained successfully.");
        }

        [HttpPost("predict-cluster")]
        public IActionResult PredictCluster([FromBody] User user)
        {
            var clusterId = _userClusteringAgent.PredictCluster(user);
            return Ok(clusterId);
        }
    }
}
