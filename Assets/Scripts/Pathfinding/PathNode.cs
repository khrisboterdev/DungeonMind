using UnityEngine;

public class PathNode
{
    public Vector2Int Position;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    public PathNode Parent;

    public PathNode(Vector2Int position)
    {
        Position = position;
        GCost = int.MaxValue;
        HCost = 0;
        Parent = null;
    }
}