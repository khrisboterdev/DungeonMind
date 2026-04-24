using System;
using UnityEngine;

public static class DungeonSettingsValidator
{
    public static DungeonSettingsException ValidateDungeonSettings(DungeonGenerationSettings settings)
    {
        if (settings.RoomHeightMin > settings.RoomHeightMax)
        {
            return new DungeonSettingsException(
                "Room Height Min must be smaller than Room Height Max");
        }
        if (settings.RoomWidthMin > settings.RoomWidthMax)
        {
            return new DungeonSettingsException(
                "Room Width Min must be smaller than Room Width Max");
        }
        if (settings.MapWidth == 0 || settings.MapHeight == 0)
        {
            return new DungeonSettingsException("Map Width/Height cannot be 0");
        }

        return null;
    }
}

[Serializable]
public class DungeonSettingsException
{
    public string Message;

    public DungeonSettingsException(string message)
    {
        Message = message;
    }    
}