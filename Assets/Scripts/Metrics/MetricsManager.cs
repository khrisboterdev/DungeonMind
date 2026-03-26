using System.Collections.Generic;
using UnityEngine;

public class MetricsManager : MonoBehaviour
{
    public static MetricsManager Instance;

    private Dictionary<DungeonAgent, RunMetrics> _currentMetrics = new();
    private int _optimalPathLength;

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
    }
}
