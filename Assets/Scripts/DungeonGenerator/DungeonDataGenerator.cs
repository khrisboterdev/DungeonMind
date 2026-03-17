using UnityEngine;

public class DungeonDataGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField] private int _mapWidth = 80;
    [SerializeField] private int _mapHeight = 50;

    [Header("Room Generation")]
    [SerializeField] private int _roomAttempts = 50;
    [SerializeField] private int _minRoomWidth = 5;
    [SerializeField] private int _maxRoomWidth = 12;
    [SerializeField] private int _minRoomHeight = 5;
    [SerializeField] private int _maxRoomHeight = 10;
    [SerializeField] private int _roomPadding = 1;

    private int _retryCount = 3;

    /// <summary>
    /// Generates the dungeon data needed to render the dungeon itself.
    /// </summary>
    /// <returns>The generated dungeon data.</returns>
    public DungeonData GenerateDungeonData()
    {
        DungeonData dungeon = new(_mapWidth, _mapHeight);

        // Main build pipeline
        FillWithEmpty(dungeon);
        GenerateRooms(dungeon);
        ConnectRooms(dungeon);
        BuildWalls(dungeon);

        if (dungeon.Rooms.Count > 0)
        {
            dungeon.StartPosition = dungeon.Rooms[0].Center;
            dungeon.ExitPosition = dungeon.Rooms[dungeon.Rooms.Count - 1].Center;

            dungeon.Tiles[dungeon.StartPosition.x, dungeon.StartPosition.y] = TileType.Start;
            dungeon.Tiles[dungeon.ExitPosition.x, dungeon.ExitPosition.y] = TileType.Exit;
        }

        if (!ValidateDungeon(dungeon) && _retryCount > 0)
        {
            GenerateDungeonData();
            _retryCount--;            
            if (_retryCount == 0)
            {
                Debug.LogError("Failed to generate dungeon, please adjust parameters.");
                return null;
            }
            Debug.Log("Generation failed, attempting again...");
        }

        Debug.Log($"Dungeon generated with {dungeon.Rooms.Count} rooms.");        
        return dungeon;
    }

    // This is just to make sure a path to the exit is possible.
    private bool ValidateDungeon(DungeonData dungeon)
    {
        return Pathfinder.FindPath(dungeon, dungeon.StartPosition, dungeon.ExitPosition) != null;
    }
    
    // Initializes the dungeon data with all empty tiles to initalize the tiles array.
    private void FillWithEmpty(DungeonData dungeon)
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                dungeon.Tiles[x, y] = TileType.Empty;
            }
        }
    }
    
    private void GenerateRooms(DungeonData dungeon)
    {
        for (int i = 0; i < _roomAttempts; i++)
        {
            int roomWidth = Random.Range(_minRoomWidth, _maxRoomWidth + 1);
            int roomHeight = Random.Range(_minRoomHeight, _maxRoomHeight + 1);

            int roomX = Random.Range(1, _mapWidth - roomWidth - 1);
            int roomY = Random.Range(1, _mapHeight - roomHeight - 1);

            Room newRoom = new Room(new RectInt(roomX, roomY, roomWidth, roomHeight));

            bool overlaps = false;
            foreach (Room existingRoom in dungeon.Rooms)
            {
                if (newRoom.Intersects(existingRoom, _roomPadding))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps)
                continue;

            dungeon.Rooms.Add(newRoom);
            CarveRoom(dungeon, newRoom);
        }
    }

    private void CarveRoom(DungeonData dungeon, Room room)
    {
        for (int x = room.Bounds.xMin; x < room.Bounds.xMax; x++)
        {
            for (int y = room.Bounds.yMin; y < room.Bounds.yMax; y++)
            {
                dungeon.Tiles[x, y] = TileType.Floor;
            }
        }
    }

    private void ConnectRooms(DungeonData dungeon)
    {
        if (dungeon.Rooms.Count < 2)
            return;

        // This is to help connect rooms that are next to eachother and not across the map.
        dungeon.Rooms.Sort((a, b) => a.Center.x.CompareTo(b.Center.x));

        for (int i = 0; i < dungeon.Rooms.Count - 1; i++)
        {
            Vector2Int currentCenter = dungeon.Rooms[i].Center;
            Vector2Int nextCenter = dungeon.Rooms[i + 1].Center;

            CarveCorridor(dungeon, currentCenter, nextCenter);

            // small possibility to create an extra hallway.
            if (i < dungeon.Rooms.Count - 3 && Random.value <= 0.25f)
            {
                nextCenter = dungeon.Rooms[i + 2].Center;

                CarveCorridor(dungeon, currentCenter, nextCenter);
            }
        }
    }

    private void CarveCorridor(DungeonData dungeon, Vector2Int start, Vector2Int end)
    {
        bool horizontalFirst = Random.value < 0.5f;

        if (horizontalFirst)
        {
            CarveHorizontalTunnel(dungeon, start.x, end.x, start.y);
            CarveVerticalTunnel(dungeon, start.y, end.y, end.x);
        }
        else
        {
            CarveVerticalTunnel(dungeon, start.y, end.y, start.x);
            CarveHorizontalTunnel(dungeon, start.x, end.x, end.y);
        }
    }

    private void CarveHorizontalTunnel(DungeonData dungeon, int xStart, int xEnd, int y)
    {
        int minX = Mathf.Min(xStart, xEnd);
        int maxX = Mathf.Max(xStart, xEnd);

        for (int x = minX; x <= maxX; x++)
        {
            bool isEndpoint = (x == xStart || x == xEnd);

            if (isEndpoint || !HasAdjacentCorridorHorizontal(dungeon, x, y))
            {
                if (dungeon.Tiles[x, y] == TileType.Empty)
                    dungeon.Tiles[x, y] = TileType.Corridor;
            }
        }
    }

    private void CarveVerticalTunnel(DungeonData dungeon, int yStart, int yEnd, int x)
    {
        int minY = Mathf.Min(yStart, yEnd);
        int maxY = Mathf.Max(yStart, yEnd);

        for (int y = minY; y <= maxY; y++)
        {
            bool isEndpoint = (y == yStart || y == yEnd);

            if (isEndpoint || !HasAdjacentCorridorVertical(dungeon, x, y))
            {
                if (dungeon.Tiles[x, y] == TileType.Empty)
                    dungeon.Tiles[x, y] = TileType.Corridor;
            }
        }
    }

    private bool HasAdjacentCorridorHorizontal(DungeonData dungeon, int x, int y)
    {
        return (IsInBounds(x, y + 1) && dungeon.Tiles[x, y + 1] == TileType.Corridor) ||
               (IsInBounds(x, y - 1) && dungeon.Tiles[x, y - 1] == TileType.Corridor);
    }

    private bool HasAdjacentCorridorVertical(DungeonData dungeon, int x, int y)
    {
        return (IsInBounds(x + 1, y) && dungeon.Tiles[x + 1, y] == TileType.Corridor) ||
               (IsInBounds(x - 1, y) && dungeon.Tiles[x - 1, y] == TileType.Corridor);
    }

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight;
    }

    private void BuildWalls(DungeonData dungeon)
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                if (!IsWalkable(dungeon.Tiles[x, y]))
                    continue;

                // Check all 8 neighbors
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (!IsInBounds(nx, ny))
                            continue;

                        if (dungeon.Tiles[nx, ny] == TileType.Empty)
                        {
                            dungeon.Tiles[nx, ny] = TileType.Wall;
                        }
                    }
                }
            }
        }
    }

    private bool IsWalkable(TileType tile)
    {
        return tile == TileType.Floor ||
               tile == TileType.Corridor ||
               tile == TileType.Start ||
               tile == TileType.Exit;
    }
}