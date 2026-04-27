using System.Collections.Generic;
using UnityEngine;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance;

    private Dictionary<DungeonAgent, RunMetrics> _currentMetrics = new();
    private int _optimalPathLength;

    private DungeonData _dungeonData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DungeonManager dungeonManager = FindFirstObjectByType<DungeonManager>();
        if (dungeonManager == null)
        {
            Debug.LogError("No Dungeon Manager found!");
            Destroy(gameObject);
        }

        dungeonManager.OnDungeonFinished += OnDungeonFinished;
    }

    private void OnDungeonFinished(DungeonData dungeon)
    {
        _dungeonData = dungeon;

        var path = Pathfinder.FindPath(dungeon, dungeon.StartPosition, dungeon.ExitPosition);
        if (path != null)
        {
            _optimalPathLength = path.Count - 1; // Not counting start tile.
            dungeon.ComplexityScore += _optimalPathLength * 0.1f;

            Debug.Log($"[Metrics Manager] Dungeon Created With Complexity Score: {dungeon.ComplexityScore}");
        }
        else
        {
            Debug.LogError("Error occurred in Metrics Manager.");
        }
    }

    public void RegisterAgent(DungeonAgent agent, RunMetrics metrics)
    {
        _currentMetrics.Add(agent, metrics);
        metrics.OptimalPathLength = _optimalPathLength;
    }

    public void AgentCompleted(DungeonAgent agent, RunMetrics metrics)
    {
        Debug.Log("Agent Finished. Printing Stats:");
        metrics.OptimalPathLength = _optimalPathLength;
        Debug.Log(metrics.ToString());

        SimulationResultIO.AppendRunResult($"{agent.name}_runs.json", ConvertRunMetricsToRunResult(metrics));
    }

    private SimulationRunResult ConvertRunMetricsToRunResult(RunMetrics metrics)
    {
        SimulationRunResult result = new();
        // agent data
        result.AgentName = metrics.AgentName;
        result.Finished = metrics.ReachedExit;
        result.FinishedTime = metrics.CompletionTime;
        result.TilesVisited = metrics.TilesVisited;
        result.TilesDiscovered = metrics.TilesDiscovered;
        result.TotalSteps = metrics.StepsTaken;
        result.BacktrackSteps = metrics.BacktrackCount;
        result.OptimalPathSteps = metrics.OptimalPathLength;
        result.EfficiencyScore = metrics.EfficiencyRatio;
        // dungeon data
        result.ComplexityScore = _dungeonData.ComplexityScore;
        result.RoomCount = _dungeonData.Rooms.Count;
        result.WalkableTileCount = _dungeonData.WalkableTiles().Count;        
        return result;
    }

    public string GetResultMetrics()
    {
        SimulationBatchResult batch = SimulationResultIO.LoadBatchResults("FogOfWarAIAgent(Clone)_runs.json");

        SimulationBatchSummary summary = SimulationBatchAnalyzer.Analyze(batch);

        string report = SimulationBatchAnalyzer.BuildReport(summary);

        return report;
    }

    public string GetCorrelationResults()
    {
        SimulationBatchResult batch = SimulationResultIO.LoadBatchResults("FogOfWarAIAgent(Clone)_runs.json");

        string report = SimulationBatchAnalyzer.BuildCorrelationReport(batch);

        return report;
    }
}
