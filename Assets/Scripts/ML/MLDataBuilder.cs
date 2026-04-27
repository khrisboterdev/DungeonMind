using System.Collections.Generic;

public static class MLDataBuilder
{
    public static List<MLDataPoint> BuildDataset(SimulationBatchResult batch)
    {
        List<MLDataPoint> dataset = new();

        foreach (var run in batch.Runs)
        {
            dataset.Add(new MLDataPoint
            {
                RoomCount = run.RoomCount,
                WalkableTileCount = run.WalkableTileCount,
                OptimalPathSteps = run.OptimalPathSteps,
                ComplexityScore = run.ComplexityScore,

                EfficiencyScore = run.EfficiencyScore
            });
        }

        return dataset;
    }
}