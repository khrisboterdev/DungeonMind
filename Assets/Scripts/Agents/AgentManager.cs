using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private GameObject _perfectAgentPrefab;    
    [SerializeField] private GameObject _fogAgentPrefab;

    private void Awake()
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
        GameObject newPerfectAgent = Instantiate(_perfectAgentPrefab);
        newPerfectAgent.GetComponent<DungeonAgent>().Initialize(dungeon);

        for (int i = 0; i < 3; i++)
        {
            GameObject newFogAgent = Instantiate(_fogAgentPrefab);
            newFogAgent.GetComponent<DungeonAgent>().Initialize(dungeon);

            if (i == 0)
                FindFirstObjectByType<DungeonViewer>().StartFollowAgent(newFogAgent.transform);
        }        
    }
}
