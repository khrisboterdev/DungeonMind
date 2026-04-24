using System;
using UnityEngine;
using TMPro;

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

    public void OnDungeonFinish(DungeonData data)
    {
        if (data == null)
            _errorText.text = "Error generating dungeon. Please try different settings.";

        _menuObject.SetActive(false);
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