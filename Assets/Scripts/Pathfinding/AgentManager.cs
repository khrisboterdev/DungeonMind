using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private int _agentCount;
    [SerializeField] private GameObject _agentPrefab;
    private List<GameObject> _currentAgents;

    private void Start()
    {
        DungeonManager dungeonManager = FindFirstObjectByType<DungeonManager>();
        if (dungeonManager == null)
        {
            Debug.LogError("No Dungeon Manager found!");
            Destroy(gameObject);
        }

        dungeonManager.OnDungeonFinished += OnDungeonFinished;
    }

    private void OnDungeonFinished(DungeonData dungeon)
    {
        _currentAgents = new();
        StartCoroutine(SpawnAgents(dungeon));
    }

    private IEnumerator SpawnAgents(DungeonData dungeon)
    {
        for (int i = 0; i < _agentCount; i++)
        {
            GameObject newAgent = Instantiate(_agentPrefab);
            newAgent.GetComponent<DungeonAgent>().Initialize(dungeon);
            _currentAgents.Add(_agentPrefab);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
