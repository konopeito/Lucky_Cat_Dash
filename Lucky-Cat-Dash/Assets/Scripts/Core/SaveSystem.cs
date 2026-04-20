using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int totalCharms;
    public int wins;
    public int losses;
    public float bestTime = 99999f;
    public float masterVolume = 1f;
    public string unlockedSkins = "default";
}

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
            return new SaveData();

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static void SaveMatchResult(float matchTime, int charmsCollected)
    {
        SaveData data = Load();
        data.totalCharms += charmsCollected;
        if (matchTime < data.bestTime) data.bestTime = matchTime;
        Save(data);
    }

    public static void RecordWin()
    {
        SaveData data = Load();
        data.wins++;
        Save(data);
    }

    public static void RecordLoss()
    {
        SaveData data = Load();
        data.losses++;
        Save(data);
    }

    public static void SaveVolume(float volume)
    {
        SaveData data = Load();
        data.masterVolume = Mathf.Clamp01(volume);
        Save(data);
    }

    public static void UnlockSkin(string skinId)
    {
        SaveData data = Load();
        if (!data.unlockedSkins.Contains(skinId))
            data.unlockedSkins += "," + skinId;
        Save(data);
    }
}
