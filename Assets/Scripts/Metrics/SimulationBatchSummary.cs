using System;

[Serializable]
public class SimulationBatchSummary
{
    public int TotalRuns;
    public int FinishedRuns;
    public float SuccessRate;

    public float AverageEfficiency;
    public float AverageCompletionTime;
    public float AverageTotalSteps;
    public float AverageBacktracks;
    public float AverageTilesVisited;
    public float AverageTilesDiscovered;
    public float AverageComplexityScore;

    public SimulationRunResult BestEfficiencyRun;
    public SimulationRunResult WorstEfficiencyRun;
    public SimulationRunResult FastestRun;
    public SimulationRunResult SlowestRun;
}