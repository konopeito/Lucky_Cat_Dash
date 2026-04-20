using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//Attempted to use/install sqlite but it caused build issues(despite following all steps), so switched to JSON file storage for simplicity and reliability across platforms.
public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    [Serializable]
    public class HighScoreEntry
    {
        public int id;
        public string playerName;
        public int score;
        public float completionTime;
    }

    [Serializable]
    private class HighScoreData
    {
        public List<HighScoreEntry> scores = new List<HighScoreEntry>();
        public int nextId = 1;
    }

    private string _savePath;
    private HighScoreData _data;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        LoadFromDisk();
        Debug.Log("High score data path: " + _savePath);
    }

    public void SaveScore(string playerName, int score, float completionTime)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        var entry = new HighScoreEntry
        {
            id = _data.nextId++,
            playerName = playerName.Trim(),
            score = score,
            completionTime = completionTime
        };

        _data.scores.Add(entry);
        SaveToDisk();
    }

    public List<HighScoreEntry> GetTopHighScores(int limit = 5)
    {
        // Return a sorted copy (score desc, then faster time asc)
        List<HighScoreEntry> sorted = new List<HighScoreEntry>(_data.scores);
        sorted.Sort((a, b) =>
        {
            int scoreCompare = b.score.CompareTo(a.score);
            if (scoreCompare != 0) return scoreCompare;
            return a.completionTime.CompareTo(b.completionTime);
        });

        if (limit < 0) limit = 0;
        if (sorted.Count > limit)
            sorted = sorted.GetRange(0, limit);

        return sorted;
    }

    public void ClearAllScores()
    {
        _data = new HighScoreData();
        SaveToDisk();
    }

    private void LoadFromDisk()
    {
        if (!File.Exists(_savePath))
        {
            _data = new HighScoreData();
            SaveToDisk(); // create file
            return;
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            _data = JsonUtility.FromJson<HighScoreData>(json);

            if (_data == null)
                _data = new HighScoreData();

            if (_data.scores == null)
                _data.scores = new List<HighScoreEntry>();

            if (_data.nextId < 1)
                _data.nextId = _data.scores.Count + 1;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to load highscores.json, creating new data. " + e.Message);
            _data = new HighScoreData();
            SaveToDisk();
        }
    }

    private void SaveToDisk()
    {
        try
        {
            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(_savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save highscores.json: " + e.Message);
        }
    }
}