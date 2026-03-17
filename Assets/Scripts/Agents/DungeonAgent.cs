using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonAgent : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 4f;
    [SerializeField] protected float _tileSize = 1f;

    public void Initialize(DungeonData dungeon)
    {
        transform.position = dungeon.GridToWorld(dungeon.StartPosition, _tileSize);
        SolveDungeon(dungeon);
    }

    protected virtual void SolveDungeon(DungeonData dungeon) { }
}