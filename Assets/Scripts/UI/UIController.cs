using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Generation Inputs")]
    [SerializeField] private TMP_InputField _mapWidthInput;
    [SerializeField] private TMP_InputField _mapHeightInput;
    [SerializeField] private TMP_InputField _roomWidthMinInput;
    [SerializeField] private TMP_InputField _roomWidthMaxInput;
    [SerializeField] private TMP_InputField _roomHeightMinInput;
    [SerializeField] private TMP_InputField _roomHeightMaxInput;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private GameObject _menuObject;
    [SerializeField] private GameObject _mainControls;
    [SerializeField] private GameObject _runResults;
    [SerializeField] private TextMeshProUGUI _runResultsText;
    [SerializeField] private Button _dataCollectionButton;
    [SerializeField] private Button _nextResultButton;

    private int _resultIndex = 0;

    public void TryDungeonGenerate()
    {
        _errorText.text = string.Empty;

        DungeonGenerationSettings newSettings = new()
        {
            MapHeight = Int32.Parse(_mapHeightInput.text),
            MapWidth = Int32.Parse(_mapWidthInput.text),
            RoomWidthMin = Int32.Parse(_roomWidthMinInput.text),
            RoomWidthMax = Int32.Parse(_roomWidthMaxInput.text),
            RoomHeightMin = Int32.Parse(_roomHeightMinInput.text),
            RoomHeightMax = Int32.Parse(_roomHeightMaxInput.text)
        };

        var dungeonException = DungeonSettingsValidator.ValidateDungeonSettings(newSettings);
        if (dungeonException != null)
        {
            _errorText.text = dungeonException.Message;
            return;
        }

        DungeonManager.Instance.OnDungeonFinished += OnDungeonFinish;
        DungeonManager.Instance.GenerateNewDungeon(newSettings);
    }

    public void StartDataCollection()
    {
        _dataCollectionButton.interactable = false;
        _runResults.gameObject.SetActive(true);
        _runResultsText.text = "Collecting Data...";

        AgentManager.Instance.SpawnPerfectAgent();
        AgentManager.Instance.SpawnAgents(100);
    }

    public void OnDungeonFinish(DungeonData data)
    {
        if (data == null)
            _errorText.text = "Error generating dungeon. Please try different settings.";

        _menuObject.SetActive(false);
        _mainControls.SetActive(true);
    }

    public void ReturnToMainMenu() 
    {
        SceneManager.LoadScene(0);
    }

    public void DisplayRunResults(string runMetricsString)
    {
        _resultIndex = 0;
        _runResultsText.text = runMetricsString;

        _nextResultButton.interactable = true;
    }

    public void ShowNextResult()
    {
        if (_resultIndex == 0)
        {
            _resultIndex = 1;
            _runResultsText.text = MetricsManager.Instance.GetCorrelationResults();
            _nextResultButton.interactable = false;
        }
    }
}

[Serializable]
public struct DungeonGenerationSettings 
{
    public int MapHeight;
    public int MapWidth;
    public int RoomWidthMin;
    public int RoomWidthMax;
    public int RoomHeightMin;
    public int RoomHeightMax;
}