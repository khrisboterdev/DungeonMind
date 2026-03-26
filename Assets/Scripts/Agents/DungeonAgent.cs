using UnityEngine;

public class DungeonAgent : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 4f;
    [SerializeField] protected float _tileSize = 1f;

    protected RunMetrics _runMetrics = new();

    public void Initialize(DungeonData dungeon)
    {
        transform.position = dungeon.GridToWorld(dungeon.StartPosition, _tileSize);
        SolveDungeon(dungeon);

        _runMetrics.AgentName = gameObject.name;

        MetricsManager.Instance.RegisterAgent(this, _runMetrics);
    }

    protected virtual void SolveDungeon(DungeonData dungeon) { }
}