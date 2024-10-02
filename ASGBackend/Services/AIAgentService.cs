using ASGBackend.Data;
using ASGBackend.Models;
using ASGBackend.Services;
using ASGShared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

public class AIAgentService
{
    private readonly GeminiService _geminiService;
    private readonly MLContext _mlContext;
    private readonly ApplicationDbContext _dbContext;
    private ITransformer _model;

    public AIAgentService(GeminiService geminiService, ApplicationDbContext dbContext)
    {
        _geminiService = geminiService;
        _mlContext = new MLContext(seed: 0);
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _dbContext.Users.Include(u => u.Preferences).ToListAsync();
    }

    public void SaveClusteringModel(ITransformer model, DataViewSchema schema)
    {
        _mlContext.Model.Save(model, schema, "user_clustering_model.zip");
    }

    public ITransformer LoadUserClusteringModel()
    {
        var modelPath = "user_clustering_model.zip";
        if (File.Exists(modelPath))
        {
            return _mlContext.Model.Load(modelPath, out var clusteringModelInputSchema);
        }
        return null;
    }

    public void SaveRecipeClusteringModel(ITransformer model, DataViewSchema schema)
    {
        _mlContext.Model.Save(model, schema, "recipe_clustering_model.zip");
    }

    public ITransformer LoadRecipeClusteringModel()
    {
        var modelPath = "recipe_clustering_model.zip";
        if (File.Exists(modelPath))
        {
            return _mlContext.Model.Load(modelPath, out var clusteringModelInputSchema);
        }
        return null;
    }

    public async Task<RecipeClassification> ClassifyRecipeAsync(Recipe recipe)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<RecipeInput, RecipeClassificationPrediction>(_model);
        var input = new RecipeInput
        {
            Name = recipe.Name,
            Ingredients = string.Join(" ", recipe.Ingredients),
            Instructions = string.Join(" ", recipe.Instructions)
        };

        var prediction = predictionEngine.Predict(input);

        var classification = new RecipeClassification
        {
            ClassificationLabel = prediction.PredictedLabel,
            Confidence = prediction.Score.Max()
        };

        // Save to database
        var classificationResult = new RecipeClassificationResult
        {
            RecipeId = recipe.Id,
            ClassificationLabel = classification.ClassificationLabel,
            Confidence = classification.Confidence,
            ClassifiedAt = DateTime.UtcNow
        };

        _dbContext.RecipeClassificationResults.Add(classificationResult);
        await _dbContext.SaveChangesAsync();

        return classification;
    }

    public async Task<List<Recipe>> GetRecipesAsync()
    {
        return await _dbContext.Recipes.ToListAsync();
    }
}
