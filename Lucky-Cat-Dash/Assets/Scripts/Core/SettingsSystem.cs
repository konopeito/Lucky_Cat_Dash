using System.IO;
using UnityEngine;

public static class SettingsSystem
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "settings.json");

    public static GameSettingsData Load()
    {
        if (!File.Exists(FilePath))
            return new GameSettingsData();

        try
        {
            string json = File.ReadAllText(FilePath);
            var data = JsonUtility.FromJson<GameSettingsData>(json);
            return data ?? new GameSettingsData();
        }
        catch
        {
            return new GameSettingsData();
        }
    }

    public static void Save(GameSettingsData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }
}