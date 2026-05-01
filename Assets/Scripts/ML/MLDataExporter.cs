using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public static class MLDataExporter
{
    public static void ExportToCSV(List<MLDataPoint> dataset, string fileName)
    {
        StringBuilder sb = new();

        // Header
        sb.AppendLine("RoomCount,WalkableTiles,OptimalPath,Complexity,Efficiency");

        foreach (var d in dataset)
        {
            sb.AppendLine(
                $"{d.RoomCount}," +
                $"{d.WalkableTileCount}," +
                $"{d.OptimalPathSteps}," +
                $"{d.ComplexityScore}," +
                $"{d.EfficiencyScore}"
            );
        }

        string dataRoot = Directory.GetParent(Application.persistentDataPath).FullName;
        string folderPath = Path.Combine(dataRoot, "MLData");

        Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllText(filePath, sb.ToString());

        Debug.Log($"Saved ML dataset to: {filePath}");
    }
}