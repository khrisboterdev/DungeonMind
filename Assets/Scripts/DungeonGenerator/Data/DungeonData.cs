using System.Collections.Generic;
using UnityEngine;

public class DungeonData
{
    public TileType[,] Tiles;
    public List<Room> Rooms;
    public Vector2Int StartPosition;
    public Vector2Int ExitPosition;
    public float ComplexityScore;

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    public DungeonData(int width, int height)
    {
        Tiles = new TileType[width, height];
        Rooms = new List<Room>();
    }

    public Vector2 GridToWorld(Vector2Int gridPosition, float tileSize)
    {
        return new Vector2(
            gridPosition.x * tileSize,
            gridPosition.y * tileSize
        );
    }

    public Vector2 GridToWorldCenter(Vector2Int gridPosition, float tileSize)
    {
        return new Vector2(
            (gridPosition.x + 0.5f) * tileSize,
            (gridPosition.y + 0.5f) * tileSize
        );
    }
}