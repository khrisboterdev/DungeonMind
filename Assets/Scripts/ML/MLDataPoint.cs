using System;

[Serializable]
public class MLDataPoint
{
    // Inputs
    public float RoomCount;
    public float WalkableTileCount;
    public float OptimalPathSteps;
    public float ComplexityScore;

    // Output
    public float EfficiencyScore;
}