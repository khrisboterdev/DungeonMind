using System.Collections.Generic;
using UnityEngine;

public static class SimulationBatchAnalyzer
{
    public static SimulationBatchSummary Analyze(SimulationBatchResult batch)
    {
        SimulationBatchSummary summary = new();

        if (batch == null || batch.Runs == null || batch.Runs.Count == 0)
        {
            Debug.LogWarning("No simulation runs found to analyze.");
            return summary;
        }

        List<SimulationRunResult> runs = batch.Runs;

        summary.TotalRuns = runs.Count;

        float efficiencyTotal = 0f;
        float timeTotal = 0f;
        float stepsTotal = 0f;
        float backtrackTotal = 0f;
        float visitedTotal = 0f;
        float discoveredTotal = 0f;
        float complexityTotal = 0f;

        foreach (SimulationRunResult run in runs)
        {
            if (run.Finished)
                summary.FinishedRuns++;

            efficiencyTotal += run.EfficiencyScore;
            timeTotal += run.FinishedTime;
            stepsTotal += run.TotalSteps;
            backtrackTotal += run.BacktrackSteps;
            visitedTotal += run.TilesVisited;
            discoveredTotal += run.TilesDiscovered;
            complexityTotal += run.ComplexityScore;

            summary.BestEfficiencyRun = GetBetterEfficiency(summary.BestEfficiencyRun, run);
            summary.WorstEfficiencyRun = GetWorseEfficiency(summary.WorstEfficiencyRun, run);
            summary.FastestRun = GetFasterRun(summary.FastestRun, run);
            summary.SlowestRun = GetSlowerRun(summary.SlowestRun, run);
        }

        summary.SuccessRate = (float)summary.FinishedRuns / summary.TotalRuns;

        summary.AverageEfficiency = efficiencyTotal / summary.TotalRuns;
        summary.AverageCompletionTime = timeTotal / summary.TotalRuns;
        summary.AverageTotalSteps = stepsTotal / summary.TotalRuns;
        summary.AverageBacktracks = backtrackTotal / summary.TotalRuns;
        summary.AverageTilesVisited = visitedTotal / summary.TotalRuns;
        summary.AverageTilesDiscovered = discoveredTotal / summary.TotalRuns;
        summary.AverageComplexityScore = complexityTotal / summary.TotalRuns;

        return summary;
    }

    public static string BuildReport(SimulationBatchSummary summary)
    {
        if (summary == null || summary.TotalRuns == 0)
            return "No simulation data available.";

        return
            "- Simulation Batch Summary -\n" +
            $"Total Runs: {summary.TotalRuns}\n" +
            $"Finished Runs: {summary.FinishedRuns}\n" +
            $"Success Rate: {summary.SuccessRate:P1}\n\n" +

            "--- Averages ---\n" +
            $"Average Efficiency: {summary.AverageEfficiency:F3}\n" +
            $"Average Completion Time: {summary.AverageCompletionTime:F2}s\n" +
            $"Average Total Steps: {summary.AverageTotalSteps:F1}\n" +
            $"Average Backtracks: {summary.AverageBacktracks:F1}\n" +
            $"Average Tiles Visited: {summary.AverageTilesVisited:F1}\n" +
            $"Average Tiles Discovered: {summary.AverageTilesDiscovered:F1}\n" +
            $"Average Complexity Score: {summary.AverageComplexityScore:F2}\n\n" +

            "--- Notable Runs ---\n" +
            FormatRun("Best Efficiency", summary.BestEfficiencyRun) +
            FormatRun("Worst Efficiency", summary.WorstEfficiencyRun) +
            FormatRun("Fastest Run", summary.FastestRun) +
            FormatRun("Slowest Run", summary.SlowestRun);
    }

    private static string FormatRun(string label, SimulationRunResult run)
    {
        if (run == null)
            return $"{label}: None\n";

        return
            $"Agent: {run.AgentName} | " +
            $"Efficiency: {run.EfficiencyScore:F3} | " +
            $"Time: {run.FinishedTime:F2}s | " +
            $"Steps: {run.TotalSteps} | " +
            $"Complexity: {run.ComplexityScore:F2}\n";
    }

    private static SimulationRunResult GetBetterEfficiency(SimulationRunResult currentBest, SimulationRunResult candidate)
    {
        if (currentBest == null)
            return candidate;

        return candidate.EfficiencyScore > currentBest.EfficiencyScore ? candidate : currentBest;
    }

    private static SimulationRunResult GetWorseEfficiency(SimulationRunResult currentWorst, SimulationRunResult candidate)
    {
        if (currentWorst == null)
            return candidate;

        return candidate.EfficiencyScore < currentWorst.EfficiencyScore ? candidate : currentWorst;
    }

    private static SimulationRunResult GetFasterRun(SimulationRunResult currentFastest, SimulationRunResult candidate)
    {
        if (currentFastest == null)
            return candidate;

        return candidate.FinishedTime < currentFastest.FinishedTime ? candidate : currentFastest;
    }

    private static SimulationRunResult GetSlowerRun(SimulationRunResult currentSlowest, SimulationRunResult candidate)
    {
        if (currentSlowest == null)
            return candidate;

        return candidate.FinishedTime > currentSlowest.FinishedTime ? candidate : currentSlowest;
    }

    private static float CalculateCorrelation(List<float> xValues, List<float> yValues)
    {
        int count = xValues.Count;

        if (count == 0 || count != yValues.Count)
            return 0f;

        float sumX = 0f;
        float sumY = 0f;
        float sumXY = 0f;
        float sumX2 = 0f;
        float sumY2 = 0f;

        for (int i = 0; i < count; i++)
        {
            float x = xValues[i];
            float y = yValues[i];

            sumX += x;
            sumY += y;
            sumXY += x * y;
            sumX2 += x * x;
            sumY2 += y * y;
        }

        float numerator = (count * sumXY) - (sumX * sumY);
        float denominator = Mathf.Sqrt(
            (count * sumX2 - sumX * sumX) *
            (count * sumY2 - sumY * sumY)
        );

        if (denominator == 0f)
            return 0f;

        return numerator / denominator;
    }

    private static List<float> Extract(List<SimulationRunResult> runs, System.Func<SimulationRunResult, float> selector)
    {
        List<float> values = new();

        foreach (var run in runs)
        {
            values.Add(selector(run));
        }

        return values;
    }

    public static string BuildCorrelationReport(SimulationBatchResult batch)
    {
        var runs = batch.Runs;

        if (runs == null || runs.Count == 0)
            return "No data for correlation.";

        float complexityVsEfficiency = CalculateCorrelation(
            Extract(runs, r => r.ComplexityScore),
            Extract(runs, r => r.EfficiencyScore)
        );

        float pathVsSteps = CalculateCorrelation(
            Extract(runs, r => r.OptimalPathSteps),
            Extract(runs, r => r.TotalSteps)
        );

        float sizeVsTime = CalculateCorrelation(
            Extract(runs, r => r.WalkableTileCount),
            Extract(runs, r => r.FinishedTime)
        );

        return
            "\n=== Correlation Analysis ===\n" +
            FormatCorrelation("Complexity vs Efficiency", complexityVsEfficiency) +
            FormatCorrelation("Optimal Path vs Total Steps", pathVsSteps) +
            FormatCorrelation("Walkable Tiles vs Completion Time", sizeVsTime);
    }

    private static string FormatCorrelation(string label, float value)
    {
        string strength =
            Mathf.Abs(value) > 0.7f ? "Strong" :
            Mathf.Abs(value) > 0.4f ? "Moderate" :
            Mathf.Abs(value) > 0.2f ? "Weak" :
            "Very Weak";

        string direction =
            value > 0 ? "Positive" :
            value < 0 ? "Negative" :
            "None";

        return $"{label}: {value:F3} ({strength} {direction})\n";
    }
}