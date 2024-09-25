using Microsoft.ML;
using ASGShared.Models;
using ASGBackend.Models;

namespace ASGBackend.Services
{
    public class AIAgentService
    {
        private readonly GeminiService _geminiService;
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public AIAgentService(GeminiService geminiService)
        {
            _geminiService = geminiService;
            _mlContext = new MLContext(seed: 0);
            // Load the pre-trained classification model
            //_model = _mlContext.Model.Load("recipe_classification_model.zip", out var modelInputSchema);
        }

        public RecipeClassification ClassifyRecipe(Recipe recipe)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<RecipeInput, RecipeClassificationPrediction>(_model);
            var input = new RecipeInput
            {
                Name = recipe.Name,
                Ingredients = string.Join(" ", recipe.Ingredients),
                Instructions = string.Join(" ", recipe.Instructions)
            };

            var prediction = predictionEngine.Predict(input);

            return new RecipeClassification
            {
                ClassificationLabel = prediction.PredictedLabel,
                Confidence = prediction.Score.Max()
            };
        }
    }
}
