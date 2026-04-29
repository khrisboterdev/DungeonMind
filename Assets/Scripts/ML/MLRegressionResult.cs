using System;

[Serializable]
public class MLRegressionResult
{
    public string model;
    public int totalRows;
    public int trainRows;
    public int testRows;
    public float meanAbsoluteError;
    public float r2Score;
    public float intercept;
    public MLCoefficient[] coefficients;
}

[Serializable]
public class MLCoefficient
{
    public string feature;
    public float value;
}