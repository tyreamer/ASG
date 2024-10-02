using ASGBackend.Data;
using ASGShared.Models;
using Google.Api;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ASGBackend.Agents
{
    public class UserClusteringAgent
    {
        private readonly MLContext _mlContext;
        private ITransformer? _model = null;
        private readonly AIAgentService _aiAgentService;
        private readonly ApplicationDbContext _context;
        private PredictionEngine<UserFeatures, ClusterPrediction> _predictionEngine;

        public UserClusteringAgent(MLContext mlContext, AIAgentService aiAgentService, ApplicationDbContext context)
        {
            _mlContext = mlContext;
            _aiAgentService = aiAgentService;
            _context = context;
        }

        private void InitializePredictionEngine()
        {
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<UserFeatures, ClusterPrediction>(_model);
        }

        public async Task TrainModelAsync()
        {
            var userFeatures = GetUserFeatures().ToList();

            if (userFeatures.Count < 3)
            {
                // Log a warning and return early
                Console.WriteLine("Warning: Not enough data points to form the required number of clusters.");
                return;
            }

            var trainingData = _mlContext.Data.LoadFromEnumerable(userFeatures);

            var pipeline = _mlContext.Transforms.Conversion.ConvertType(new[]
                            {
                                new InputOutputColumnPair("IsVegetarian", "IsVegetarian"),
                                new InputOutputColumnPair("IsVegan", "IsVegan"),
                                new InputOutputColumnPair("IsGlutenFree", "IsGlutenFree"),
                                new InputOutputColumnPair("IsPescatarian", "IsPescatarian"),
                                new InputOutputColumnPair("HighProtein", "HighProtein"),
                                new InputOutputColumnPair("LowCarb", "LowCarb"),
                                new InputOutputColumnPair("LowFat", "LowFat")
                            }, DataKind.Single)
                            .Append(_mlContext.Transforms.Concatenate("Features",
                                "HouseholdSize",
                                "TotalTimeConstraintInMinutes",
                                "CookingSkillLevel",
                                "TotalMealsPerWeek",
                                "CalorieTarget",
                                "IsVegetarian",
                                "IsVegan",
                                "IsGlutenFree",
                                "IsPescatarian",
                                "HighProtein",
                                "LowCarb",
                                "LowFat"))
                            .Append(_mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: Math.Min(3, userFeatures.Count)));

            _model = pipeline.Fit(trainingData);

            InitializePredictionEngine();
        }

        public int Predict(UserFeatures userFeatures)
        {
            var prediction = _predictionEngine.Predict(userFeatures);
            return (int)prediction.PredictedClusterId;
        }
        private IQueryable<UserFeatures> GetUserFeatures()
        {
            return _context.Users.Select(user => new UserFeatures
            {
                HouseholdSize = user.HouseholdSize,
                TotalTimeConstraintInMinutes = user.TotalTimeConstraintInMinutes,
                CookingSkillLevel = user.CookingSkillLevel,
                TotalMealsPerWeek = user.Preferences.TotalMealsPerWeek,
                CalorieTarget = user.Preferences.CalorieTarget,
                IsVegetarian = user.Preferences.DietaryRestrictions.IsVegetarian,
                IsVegan = user.Preferences.DietaryRestrictions.IsVegan,
                IsGlutenFree = user.Preferences.DietaryRestrictions.IsGlutenFree,
                IsPescatarian = user.Preferences.DietaryRestrictions.IsPescatarian,
                HighProtein = user.Preferences.NutritionalGoals.HighProtein,
                LowCarb = user.Preferences.NutritionalGoals.LowCarb,
                LowFat = user.Preferences.NutritionalGoals.LowFat
            });
        }
    }

    public class UserFeatures
    {
        public float HouseholdSize { get; set; }
        public float TotalTimeConstraintInMinutes { get; set; }
        public float CookingSkillLevel { get; set; }
        public float TotalMealsPerWeek { get; set; }
        public float CalorieTarget { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsPescatarian { get; set; }
        public bool HighProtein { get; set; }
        public bool LowCarb { get; set; }
        public bool LowFat { get; set; }
    }

    public class ClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId { get; set; }
    }
}
