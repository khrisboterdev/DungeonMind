using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class PythonRegressionRunner
{
    private const string PYTHON_EXE = "python";

    private const string PYTHON_SCRIPT_LOCATION = "Assets/Scripts/ML/py/train_regression.py";
    private const string DATA_FOLDER_NAME = "MLData";
    private const string INPUT_CSV = "ml_dataset.csv";
    private const string OUTPUT_JSON = "ml_results.json";

    public static void RunRegression()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;

        string scriptPath = Path.Combine(projectRoot, PYTHON_SCRIPT_LOCATION);
        string dataFolder = Path.Combine(projectRoot, "Assets", DATA_FOLDER_NAME);

        Directory.CreateDirectory(dataFolder);

        string inputCsvPath = Path.Combine(projectRoot, "Assets", DATA_FOLDER_NAME, INPUT_CSV);
        string outputJsonPath = Path.Combine(projectRoot, "Assets/OutputJSON", OUTPUT_JSON);

        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError($"Python script not found: {scriptPath}");
            return;
        }

        if (!File.Exists(inputCsvPath))
        {
            UnityEngine.Debug.LogError($"Input CSV not found: {inputCsvPath}");
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = PYTHON_EXE,
            Arguments = $"\"{scriptPath}\" \"{inputCsvPath}\" \"{outputJsonPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using Process process = new Process();
        process.StartInfo = startInfo;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrWhiteSpace(output))
            UnityEngine.Debug.Log(output);

        if (!string.IsNullOrWhiteSpace(errors))
            UnityEngine.Debug.LogWarning(errors);

        if (File.Exists(outputJsonPath))
        {
            string resultJson = File.ReadAllText(outputJsonPath);
            UnityEngine.Debug.Log($"Regression results saved:\n{resultJson}");
        }
    }
}