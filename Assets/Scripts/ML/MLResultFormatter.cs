using System.IO;
using System.Text;
using UnityEngine;

public static class MLResultFormatter
{    
    private static MLRegressionResult LoadResults()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string folderPath = Path.Combine(projectRoot, "Assets");
        string filePath = Path.Combine(folderPath, "ml_results.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"ML results file not found: {folderPath}");
            return null;
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<MLRegressionResult>(json);
    }

    public static string BuildDisplayText()
    {
        MLRegressionResult result = LoadResults();

        if (result == null)
            return "No ML results available.";

        StringBuilder sb = new();

        sb.AppendLine("Machine Learning Results");
        sb.AppendLine("------------------------");
        sb.AppendLine($"Model: {result.model}");
        sb.AppendLine($"Dataset Rows: {result.totalRows}");
        sb.AppendLine($"Training Rows: {result.trainRows}");
        sb.AppendLine($"Testing Rows: {result.testRows}");
        sb.AppendLine();
        sb.AppendLine($"Mean Absolute Error: {result.meanAbsoluteError:F3}");
        sb.AppendLine($"R² Score: {result.r2Score:F3}");
        sb.AppendLine();

        sb.AppendLine(GetModelQualityText(result));
        sb.AppendLine();

        sb.AppendLine("Feature Impact:");
        foreach (var coef in result.coefficients)
        {
            sb.AppendLine($"{coef.feature}: {FormatCoefficient(coef.value)}");
        }

        return sb.ToString();
    }

    private static string GetModelQualityText(MLRegressionResult result)
    {
        if (result.r2Score >= 0.7f)
            return "Model Fit: Strong predictive relationship found.";

        if (result.r2Score >= 0.4f)
            return "Model Fit: Moderate predictive relationship found.";

        if (result.r2Score >= 0.2f)
            return "Model Fit: Weak predictive relationship found.";

        return "Model Fit: Low predictive relationship. More data or better features may be needed.";
    }

    private static string FormatCoefficient(float value)
    {
        string direction = value >= 0 ? "increases predicted efficiency" : "decreases predicted efficiency";
        return $"{value:F4} ({direction})";
    }

    public enum DungeonDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public static DungeonDifficulty GetDifficulty(float efficiency)
    {
        if (efficiency >= 0.5f)
            return DungeonDifficulty.Easy;

        if (efficiency >= 0.25f)
            return DungeonDifficulty.Medium;

        return DungeonDifficulty.Hard;
    }

    public static string BuildDungeonPredictionText(DungeonData dungeon)
    {
        MLRegressionResult result = LoadResults();

        if (result == null)
            return "No ML results available.";

        float predictedEfficiency = PredictEfficiency(result, dungeon);
        DungeonDifficulty difficulty = GetDifficulty(predictedEfficiency);

        StringBuilder sb = new();

        sb.AppendLine("Dungeon Prediction");
        sb.AppendLine("------------------");
        sb.AppendLine($"Predicted Efficiency: {predictedEfficiency:F3}");
        sb.AppendLine($"Predicted Difficulty: {difficulty}");
        sb.AppendLine(GetDifficultyDescription(difficulty));

        return sb.ToString();
    }

    private static float PredictEfficiency(MLRegressionResult result, DungeonData dungeon)
    {
        float prediction = result.intercept;

        foreach (MLCoefficient coef in result.coefficients)
        {
            float featureValue = GetDungeonFeatureValue(coef.feature, dungeon);
            prediction += coef.value * featureValue;
        }

        return Mathf.Clamp01(prediction);
    }

    private static float GetDungeonFeatureValue(string featureName, DungeonData dungeon)
    {
        return featureName switch
        {
            "RoomCount" => dungeon.Rooms.Count,
            "Complexity" => dungeon.ComplexityScore,
            _ => 0f
        };
    }

    private static string GetDifficultyDescription(DungeonDifficulty difficulty)
    {
        return difficulty switch
        {
            DungeonDifficulty.Easy => "Easy: The agent is expected to solve this dungeon efficiently.",
            DungeonDifficulty.Medium => "Medium: Some exploration and backtracking is expected.",
            DungeonDifficulty.Hard => "Hard: Significant exploration and backtracking is expected.",
            _ => "Unknown difficulty."
        };
    }
}