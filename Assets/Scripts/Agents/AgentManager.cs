using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance;

    [SerializeField] private UIController _uiController;
    [SerializeField] private GameObject _perfectAgentPrefab;    
    [SerializeField] private GameObject _fogAgentPrefab;

    private DungeonData _currentDungeon;
    private List<DungeonAgent> _currentAgents = new();

    private void Awake()
    {
        Instance = this;

        DungeonManager dungeonManager = FindFirstObjectByType<DungeonManager>();
        if (dungeonManager == null)
        {
            Debug.LogError("No Dungeon Manager found!");
            Destroy(gameObject);
        }

        dungeonManager.OnDungeonFinished += OnDungeonFinished;
    }

    private void OnDungeonFinished(DungeonData data)
    {
        _currentDungeon = data;

        if (_currentAgents.Count > 0)
        {
            for (int i = 0; i < _currentAgents.Count; i++)
            {
                Destroy(_currentAgents[i]);
            }
        }

        _currentAgents.Clear();
        _currentAgents = new();
    }

    public void SpawnAgents(int numAgents)
    {
        for (int i = 0; i < numAgents; i++)
        {
            GameObject newFogAgent = Instantiate(_fogAgentPrefab);
            newFogAgent.GetComponent<DungeonAgent>().Initialize(_currentDungeon);
            _currentAgents.Add(newFogAgent.GetComponent<DungeonAgent>());
        }
    }

    public void SpawnPerfectAgent()
    {
        GameObject newPerfectAgent = Instantiate(_perfectAgentPrefab);
        newPerfectAgent.GetComponent<DungeonAgent>().Initialize(_currentDungeon);
        _currentAgents.Add(newPerfectAgent.GetComponent<DungeonAgent>());
    }

    public void AgentReportDone(DungeonAgent agent)
    {
        _currentAgents.Remove(agent);
        Destroy(agent.gameObject);

        if (_currentAgents.Count == 0)
        {
            _uiController.DisplayRunResults(MetricsManager.Instance.GetResultMetrics());
        }
    }
}
