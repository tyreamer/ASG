using System.Text;
using Newtonsoft.Json;
using Microsoft.ML;
using Microsoft.ML.Data;
using ASGShared.Models;

namespace ASGBackend.Services
{
    public class AIAgentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAIApiKey;
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public AIAgentService(HttpClient httpClient, string openAIApiKey)
        {
            _httpClient = httpClient;
            _openAIApiKey = openAIApiKey;
            _mlContext = new MLContext(seed: 0);
            // Load the pre-trained classification model
            //_model = _mlContext.Model.Load("recipe_classification_model.zip", out var modelInputSchema);
        }

        public async Task<string> GenerateRecipe(UserPreferences preferences, Budget budget)
        {
            var prompt = $"Generate a recipe for a user with the following preferences: " +
                         $"Dietary restrictions: {preferences.DietaryRestrictions}, " +
                         $"Cuisine preferences: {preferences.FavoriteCuisines}, " +
                         $"Calorie target: {preferences.CalorieTarget}, " +
                         $"Budget per meal: {budget.Amount} {budget.Currency}";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are a helpful assistant that generates recipes based on user preferences." },
                new { role = "user", content = prompt }
            }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = content
            };
            request.Headers.Add("Authorization", $"Bearer {_openAIApiKey}");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<OpenAIResponse>(responseBody);

            return responseObject.Choices[0].Message.Content;
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

    public class RecipeInput
    {
        [LoadColumn(0)]
        public string Name { get; set; }

        [LoadColumn(1)]
        public string Ingredients { get; set; }

        [LoadColumn(2)]
        public string Instructions { get; set; }
    }

    public class RecipeClassificationPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; }

        [ColumnName("Score")]
        public float[] Score { get; set; }
    }

    public class RecipeClassification
    {
        public string ClassificationLabel { get; set; }
        public float Confidence { get; set; }
    }
}
