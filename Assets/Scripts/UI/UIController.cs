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

    private void Start()
    {
        LoadPlayerPrefs();
    }

    private void LoadPlayerPrefs()
    {
        Debug.Log("Loading previous dungeon values...");

        _mapWidthInput.text = PlayerPrefs.GetInt("MapWidth", 80).ToString();
        _mapHeightInput.text = PlayerPrefs.GetInt("MapHeight", 50).ToString();
        _roomWidthMinInput.text = PlayerPrefs.GetInt("RoomWidthMin", 4).ToString();
        _roomWidthMaxInput.text = PlayerPrefs.GetInt("RoomWidthMax", 24).ToString();
        _roomHeightMinInput.text = PlayerPrefs.GetInt("RoomHeightMin", 3).ToString();
        _roomHeightMaxInput.text = PlayerPrefs.GetInt("RoomHeightMax", 20).ToString();
    }

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

        PlayerPrefs.SetInt("MapHeight", newSettings.MapHeight);
        PlayerPrefs.SetInt("MapWidth", newSettings.MapWidth);
        PlayerPrefs.SetInt("RoomWidthMin", newSettings.RoomWidthMin);
        PlayerPrefs.SetInt("RoomWidthMax", newSettings.RoomWidthMax);
        PlayerPrefs.SetInt("RoomHeightMin", newSettings.RoomHeightMin);
        PlayerPrefs.SetInt("RoomHeightMax", newSettings.RoomHeightMax);

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
            _nextResultButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start ML Regression";
            return;

        }
        else if (_resultIndex == 1)
        {
            _resultIndex = 2;
            _nextResultButton.interactable = false;

            var batch = SimulationResultIO.LoadBatchResults("FogOfWarAIAgent(Clone)_runs.json");
            var dataset = MLDataBuilder.BuildDataset(batch);
            MLDataExporter.ExportToCSV(dataset, "ml_dataset.csv");

            PythonRegressionRunner.RunRegression();
            var result = MLResultFormatter.BuildDisplayText();
            _runResultsText.text = result;
            _nextResultButton.interactable = true;
            _nextResultButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show interpreted results";
        }
        else if (_resultIndex == 2)
        {
            _nextResultButton.interactable = false;
            _runResultsText.text = MLResultFormatter.BuildDungeonPredictionText(
                DungeonManager.Instance.CurrentDungeon);
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