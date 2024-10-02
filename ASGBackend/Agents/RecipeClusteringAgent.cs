using ASGShared.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASGBackend.Agents
{
    public class RecipeClusteringAgent
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private readonly AIAgentService _aiAgentService;
        private PredictionEngine<RecipeFeatures, ClusterPrediction> _predictionEngine;

        public RecipeClusteringAgent(AIAgentService aiAgentService)
        {
            _mlContext = new MLContext(seed: 0);
            _aiAgentService = aiAgentService;
            _model = _aiAgentService.LoadRecipeClusteringModel();
            InitializePredictionEngine();
        }

        private void InitializePredictionEngine()
        {
            if (_model != null)
            {
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<RecipeFeatures, ClusterPrediction>(_model);
            }
        }

        public async Task TrainModelAsync()
        {
            var recipes = await _aiAgentService.GetRecipesAsync();
            var data = recipes.Select(recipe => new RecipeFeatures
            {
                Calories = recipe.Calories,
                CookingTime = recipe.CookingTime,
                CuisineType = recipe.CuisineType,
                Ingredients = recipe.Ingredients.Length,
                Instructions = recipe.Instructions.Length
            }).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("CuisineType")
                .Append(_mlContext.Transforms.Concatenate("Features", "Calories", "CookingTime", "CuisineType", "Ingredients", "Instructions"))
                .Append(_mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 3));

            _model = pipeline.Fit(dataView);

            // Save the model using AIAgentService
            _aiAgentService.SaveRecipeClusteringModel(_model, dataView.Schema);

            // Reinitialize the prediction engine with the new model
            InitializePredictionEngine();
        }

        public int PredictCluster(Recipe recipe)
        {
            if (_model == null)
            {
                throw new InvalidOperationException("Model has not been trained.");
            }

            var recipeFeatures = new RecipeFeatures
            {
                Calories = recipe.Calories,
                CookingTime = recipe.CookingTime,
                CuisineType = recipe.CuisineType,
                Ingredients = recipe.Ingredients.Length,
                Instructions = recipe.Instructions.Length
            };

            var prediction = _predictionEngine.Predict(recipeFeatures);
            return (int)prediction.PredictedClusterId;
        }
    }

    public class RecipeFeatures
    {
        public string Calories { get; set; }
        public string CookingTime { get; set; }
        public string CuisineType { get; set; }
        public int Ingredients { get; set; }
        public int Instructions { get; set; }
    }
}
