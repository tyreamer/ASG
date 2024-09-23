using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASGShared.Models;
using Google.Api;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ASGBackend.Agents
{
    public class UserClusteringAgent
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public UserClusteringAgent()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<User> users)
        {
            var data = users.Select(user => new UserFeatures
            {
                DietaryRestrictions = string.Join(",", user.Preferences.DietaryRestrictions),
                CuisinePreferences = string.Join(",", user.Preferences.FavoriteCuisines),
                BudgetPerMeal = (float)user.BudgetPerMeal.Amount, // Changed to float
                HouseholdSize = user.HouseholdSize,
                CookingSkillLevel = (float)user.CookingSkillLevel
            }).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("DietaryRestrictionsFeaturized", "DietaryRestrictions")
                .Append(_mlContext.Transforms.Text.FeaturizeText("CuisinePreferencesFeaturized", "CuisinePreferences"))
                .Append(_mlContext.Transforms.Concatenate("Features", "DietaryRestrictionsFeaturized", "CuisinePreferencesFeaturized", "WeeklyBudget", "HouseholdSize", "CookingSkillLevel"))
                .Append(_mlContext.Clustering.Trainers.KMeans(numberOfClusters: 5));

            _model = pipeline.Fit(dataView);
        }

        public int PredictCluster(User user)
        {
            var predictor = _mlContext.Model.CreatePredictionEngine<UserFeatures, ClusterPrediction>(_model);

            var prediction = predictor.Predict(new UserFeatures
            {
                DietaryRestrictions = string.Join(",", user.Preferences.DietaryRestrictions),
                CuisinePreferences = string.Join(",", user.Preferences.FavoriteCuisines),
                BudgetPerMeal = (float)user.BudgetPerMeal.Amount,
                HouseholdSize = user.HouseholdSize,
                CookingSkillLevel = (float)user.CookingSkillLevel
            });

            return (int)prediction.PredictedClusterId;
        }
    }

    public class UserFeatures
    {
        public string DietaryRestrictions { get; set; } = string.Empty;
        public string CuisinePreferences { get; set; } = string.Empty;
        public float BudgetPerMeal { get; set; }
        public int HouseholdSize { get; set; }
        public float CookingSkillLevel { get; set; }
    }

    public class ClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;
    }
}
