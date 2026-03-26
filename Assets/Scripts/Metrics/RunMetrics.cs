using System.Text;
using UnityEngine;

[System.Serializable]
public class RunMetrics
{
    public string AgentName;
    public bool ReachedExit;
    public int StepsTaken;
    public int TilesVisited;
    public int TilesDiscovered;
    public int BacktrackCount;
    public int OptimalPathLength;
    public float CompletionTime;

    public float EfficiencyRatio
    {
        get
        {
            if (OptimalPathLength <= 0 || StepsTaken <= 0)
                return 0f;

            return (float)OptimalPathLength / StepsTaken;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(AgentName);
        sb.AppendLine($"Finished: {ReachedExit}");
        if (ReachedExit)
        {
            sb.AppendLine($"Finished Time: {CompletionTime}s");
        }
        sb.AppendLine($"Tiles Visted: {TilesVisited}");
        sb.AppendLine($"Tiles Discovered: {TilesDiscovered}");
        sb.AppendLine($"Total Steps: {StepsTaken}");
        sb.AppendLine($"Backtrack Steps: {BacktrackCount}");
        sb.AppendLine($"Optimal Path Steps: {OptimalPathLength}");
        sb.AppendLine($"Efficiency Score: {EfficiencyRatio}");
        return sb.ToString();
    }
}
