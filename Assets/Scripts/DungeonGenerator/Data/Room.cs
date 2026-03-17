using UnityEngine;

[System.Serializable]
public class Room
{
    public RectInt Bounds;

    public Vector2Int Center => new Vector2Int(
        Bounds.x + Bounds.width / 2,
        Bounds.y + Bounds.height / 2
    );

    public Room(RectInt bounds)
    {
        Bounds = bounds;
    }

    public bool Intersects(Room other, int padding = 1)
    {
        RectInt expanded = new RectInt(
            Bounds.xMin - padding,
            Bounds.yMin - padding,
            Bounds.width + padding * 2,
            Bounds.height + padding * 2
        );

        return expanded.Overlaps(other.Bounds);
    }
}