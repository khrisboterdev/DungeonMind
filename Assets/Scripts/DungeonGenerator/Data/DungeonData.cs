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

    public List<TileType> WalkableTiles()
    {
        List<TileType> result = new();

        for (int i = 0; i < Tiles.GetLength(0); i++)
        {
            for (int j = 0; j < Tiles.GetLength(1); j++)
            {
                var tile = Tiles[i, j];
                switch (tile)
                {
                    case TileType.Floor:
                    case TileType.Corridor:
                    case TileType.Exit:
                    case TileType.Start:
                        result.Add(tile);
                        break;
                }
            }
        }

        return result;
    }
}