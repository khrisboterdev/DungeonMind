using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class uses the A* algorithm for finding the best possible route to the dungeon exit.
/// </summary>
public static class Pathfinder
{
    public static List<Vector2Int> FindPath(DungeonData dungeon, Vector2Int start, Vector2Int goal)
    {
        int width = dungeon.Width;
        int height = dungeon.Height;

        PathNode[,] nodes = new PathNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = new PathNode(new Vector2Int(x, y));
            }
        }

        PathNode startNode = nodes[start.x, start.y];
        PathNode goalNode = nodes[goal.x, goal.y];

        List<PathNode> openList = new();
        HashSet<PathNode> closedList = new();

        startNode.GCost = 0;
        startNode.HCost = GetManhattanDistance(start, goal);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == goalNode)
            {
                return BuildPath(goalNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Vector2Int neighborPos in GetNeighbors(currentNode.Position, width, height))
            {
                PathNode neighborNode = nodes[neighborPos.x, neighborPos.y];

                if (closedList.Contains(neighborNode))
                    continue;

                if (!IsWalkable(dungeon.Tiles[neighborPos.x, neighborPos.y]))
                    continue;

                int tentativeGCost = currentNode.GCost + 1;

                if (tentativeGCost < neighborNode.GCost)
                {
                    neighborNode.Parent = currentNode;
                    neighborNode.GCost = tentativeGCost;
                    neighborNode.HCost = GetManhattanDistance(neighborPos, goal);

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        return null;
    }

    private static List<Vector2Int> BuildPath(PathNode goalNode)
    {
        List<Vector2Int> path = new();
        PathNode current = goalNode;

        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    private static PathNode GetLowestFCostNode(List<PathNode> nodes)
    {
        PathNode bestNode = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].FCost < bestNode.FCost ||
                (nodes[i].FCost == bestNode.FCost && nodes[i].HCost < bestNode.HCost))
            {
                bestNode = nodes[i];
            }
        }

        return bestNode;
    }

    private static IEnumerable<Vector2Int> GetNeighbors(Vector2Int position, int width, int height)
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = position + dir;

            if (neighbor.x >= 0 && neighbor.x < width &&
                neighbor.y >= 0 && neighbor.y < height)
            {
                yield return neighbor;
            }
        }
    }

    private static int GetManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static bool IsWalkable(TileType tile)
    {
        return tile == TileType.Floor ||
               tile == TileType.Corridor ||
               tile == TileType.Start ||
               tile == TileType.Exit;
    }
}
