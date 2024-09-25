using Microsoft.ML.Data;

namespace ASGBackend.Models
{
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
