using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogExplorerAgent : DungeonAgent
{    
    [SerializeField] private float _pauseAtTile = 0.05f;

    [Header("Vision")]
    [SerializeField] private int _revealRadius = 2;

    private DungeonData _currentDungeon;
    private Vector2Int _currentGridPosition;

    private readonly HashSet<Vector2Int> _visitedTiles = new();
    private readonly Dictionary<Vector2Int, TileType> _discoveredTiles = new();
    private readonly Stack<Vector2Int> _pathStack = new();

    private Coroutine _exploreRoutine;
    private bool _foundExit = false;

    protected override void SolveDungeon(DungeonData dungeon)
    {
        _currentDungeon = dungeon;
        _currentGridPosition = dungeon.StartPosition;

        _visitedTiles.Clear();
        _discoveredTiles.Clear();
        _pathStack.Clear();
        _foundExit = false;

        transform.position = dungeon.GridToWorld(_currentGridPosition, _tileSize);

        RevealAround(_currentGridPosition);
        _visitedTiles.Add(_currentGridPosition);
        _pathStack.Push(_currentGridPosition);

        if (_exploreRoutine != null)
        {
            StopCoroutine(_exploreRoutine);
        }

        _exploreRoutine = StartCoroutine(ExploreRoutine());
    }

    private IEnumerator ExploreRoutine()
    {
        while (!_foundExit)
        {
            if (_currentGridPosition == _currentDungeon.ExitPosition)
            {
                _foundExit = true;
                Debug.Log("Explorer found the exit.");
                yield break;
            }

            Vector2Int? nextMove = GetNextMove();

            if (nextMove.HasValue)
            {
                yield return MoveTo(nextMove.Value);
            }
            else
            {
                Debug.Log("Explorer could not find a valid move.");
                yield break;
            }
        }
    }

    private Vector2Int? GetNextMove()
    {
        List<Vector2Int> neighbors = GetDiscoveredWalkableNeighbors(_currentGridPosition);

        // 1. Prefer any unvisited neighbor
        foreach (Vector2Int neighbor in neighbors)
        {
            if (!_visitedTiles.Contains(neighbor))
            {
                _pathStack.Push(neighbor);
                return neighbor;
            }
        }

        // 2. No new neighbors? Backtrack
        if (_pathStack.Count > 1)
        {
            _pathStack.Pop(); // remove current
            return _pathStack.Peek(); // go back
        }

        return null;
    }

    private IEnumerator MoveTo(Vector2Int destination)
    {
        Vector3 targetPosition = _currentDungeon.GridToWorld(destination, _tileSize);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        _currentGridPosition = destination;

        if (!_visitedTiles.Contains(destination))
        {
            _visitedTiles.Add(destination);
        }

        RevealAround(_currentGridPosition);

        yield return new WaitForSeconds(_pauseAtTile);
    }

    private void RevealAround(Vector2Int center)
    {
        for (int dx = -_revealRadius; dx <= _revealRadius; dx++)
        {
            for (int dy = -_revealRadius; dy <= _revealRadius; dy++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);

                if (!IsInBounds(pos))
                    continue;

                _discoveredTiles[pos] = _currentDungeon.Tiles[pos.x, pos.y];
            }
        }
    }

    private List<Vector2Int> GetDiscoveredWalkableNeighbors(Vector2Int position)
    {
        List<Vector2Int> results = new();

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        foreach (Vector2Int dir in GetShuffledDirections())
        {
            Vector2Int neighbor = position + dir;

            if (!_discoveredTiles.ContainsKey(neighbor))
                continue;

            TileType tile = _discoveredTiles[neighbor];

            if (IsWalkable(tile))
            {
                results.Add(neighbor);
            }
        }

        return results;
    }

    private List<Vector2Int> GetShuffledDirections()
    {
        List<Vector2Int> directions = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

        for (int i = 0; i < directions.Count; i++)
        {
            int randomIndex = Random.Range(i, directions.Count);
            (directions[i], directions[randomIndex]) = (directions[randomIndex], directions[i]);
        }

        return directions;
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 &&
               pos.x < _currentDungeon.Width &&
               pos.y >= 0 &&
               pos.y < _currentDungeon.Height;
    }

    private bool IsWalkable(TileType tile)
    {
        return tile == TileType.Floor ||
               tile == TileType.Corridor ||
               tile == TileType.Start ||
               tile == TileType.Exit;
    }
}