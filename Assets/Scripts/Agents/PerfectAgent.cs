using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PerfectAgent : DungeonAgent
{
    private List<Vector2Int> _currentPath;
    private Coroutine _moveRoutine;

    protected override void SolveDungeon(DungeonData dungeon)
    {
        _currentPath = Pathfinder.FindPath(dungeon, dungeon.StartPosition, dungeon.ExitPosition);

        if (_currentPath == null || _currentPath.Count == 0)
        {
            Debug.LogWarning("No path found.");
            return;
        }

        Vector3 startWorld = GridToWorld(_currentPath[0]);
        transform.position = startWorld;

        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
        }

        _moveRoutine = StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        for (int i = 1; i < _currentPath.Count; i++)
        {
            Vector3 targetPosition = GridToWorld(_currentPath[i]);

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
        }

        Debug.Log("Agent reached the exit.");
        Destroy(gameObject);
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * _tileSize, gridPosition.y * _tileSize, 0f);
    }
}
