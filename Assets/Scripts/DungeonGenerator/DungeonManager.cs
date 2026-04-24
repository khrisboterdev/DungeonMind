using UnityEngine;

[RequireComponent(typeof(DungeonDataGenerator))]
public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;
    private DungeonDataGenerator _generator;

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private GameObject _emptyPrefab;
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _startPrefab;
    [SerializeField] private GameObject _exitPrefab;

    [Header("Settings")]
    [SerializeField] private Transform _tileParent;
    [SerializeField] private float _tileSize = 1f;

    public DungeonData CurrentDungeon { get; private set; }

    public delegate void DungeonFinished(DungeonData dungeon);
    public event DungeonFinished OnDungeonFinished;

    private void Awake()
    {
        Instance = this;
        _generator = GetComponent<DungeonDataGenerator>();
    }

    public void GenerateNewDungeon(DungeonGenerationSettings settings)
    {
        GenerateAndRender(settings);
        OnDungeonFinished?.Invoke(CurrentDungeon);
    }

    public void GenerateAndRender(DungeonGenerationSettings settings)
    {
        ClearOldTiles();

        CurrentDungeon = _generator.GenerateDungeonData();

        for (int x = 0; x < CurrentDungeon.Width; x++)
        {
            for (int y = 0; y < CurrentDungeon.Height; y++)
            {
                GameObject prefabToSpawn = GetTilePrefabByTileType(CurrentDungeon.Tiles[x, y]);

                if (prefabToSpawn != null)
                {
                    Vector3 position = new Vector3(x * _tileSize, y * _tileSize, 0f);
                    Instantiate(prefabToSpawn, position, Quaternion.identity, _tileParent);
                }
            }
        }
    }

    private GameObject GetTilePrefabByTileType(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Corridor:
            case TileType.Floor:
                return _floorPrefab;
            case TileType.Start:
                return _startPrefab;
            case TileType.Exit:
                return _exitPrefab;
            case TileType.Wall:
                return _wallPrefab;
            default:
            case TileType.Empty:
                return _emptyPrefab;
        }
    }

    private void ClearOldTiles()
    {
        Transform parent = _tileParent != null ? _tileParent : transform;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
