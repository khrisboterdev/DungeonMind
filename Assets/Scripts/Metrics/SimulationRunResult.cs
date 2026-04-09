using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SimulationRunResult
{
    public string AgentName;

    public bool Finished;
    public float FinishedTime;

    public int TilesVisited;
    public int TilesDiscovered;
    public int TotalSteps;
    public int BacktrackSteps;
    public int OptimalPathSteps;
    public float EfficiencyScore;

    public float ComplexityScore;
    public int RoomCount;
    public int WalkableTileCount;
}

[Serializable]
public class SimulationBatchResult
{
    public string BatchName;
    public List<SimulationRunResult> Runs = new();
}

public static class SimulationResultIO
{
    private static string GetFolderPath()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "SimulationResults");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return folderPath;
    }

    private static string GetFilePath(string fileName)
    {
        return Path.Combine(GetFolderPath(), fileName);
    }

    public static SimulationBatchResult LoadBatchResults(string fileName)
    {
        string filePath = GetFilePath(fileName);

        if (!File.Exists(filePath))
        {
            Debug.Log($"No existing results file found. Creating new dataset: {fileName}");

            return new SimulationBatchResult
            {
                BatchName = fileName
            };
        }

        string json = File.ReadAllText(filePath);

        SimulationBatchResult result =
            JsonUtility.FromJson<SimulationBatchResult>(json);

        if (result == null)
        {
            Debug.LogWarning("JSON file existed but failed to parse. Creating new dataset.");

            result = new SimulationBatchResult
            {
                BatchName = fileName
            };
        }

        return result;
    }

    public static void SaveBatchResults(SimulationBatchResult batchResult, string fileName)
    {
        string json = JsonUtility.ToJson(batchResult, true);

        string filePath = GetFilePath(fileName);

        File.WriteAllText(filePath, json);

        Debug.Log($"Saved simulation results to: {filePath}");
    }

    public static void AppendRunResult(string fileName, SimulationRunResult run)
    {
        SimulationBatchResult batch = LoadBatchResults(fileName);

        batch.Runs.Add(run);

        SaveBatchResults(batch, fileName);
    }
}