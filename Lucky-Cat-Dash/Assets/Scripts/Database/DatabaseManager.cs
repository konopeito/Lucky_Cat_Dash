using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using SQLite;         
using SQLitePCL;     

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

    private SQLiteConnection _db;
    private string _dbPath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    private void OnDestroy()
    {
        if (_db != null)
        {
            _db.Close();
            _db = null;
        }
    }

    private void InitializeDatabase()
    {
       
        Batteries_V2.Init();

        _dbPath = Path.Combine(Application.persistentDataPath, "highscores.db");
        _db = new SQLiteConnection(_dbPath);

        CreateHighScoresTable();

        Debug.Log("[DatabaseManager] SQLite DB path: " + _dbPath);
    }

    // Step 4: Create table (SQL)
    private void CreateHighScoresTable()
    {
        
        _db.Execute(@"
            CREATE TABLE IF NOT EXISTS HighScores (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                playerName TEXT NOT NULL,
                score INTEGER NOT NULL,
                completionTime REAL NOT NULL
            );
        ");

        _db.Execute(@"
            CREATE INDEX IF NOT EXISTS idx_HighScores_score_time
            ON HighScores(score DESC, completionTime ASC);
        ");
    }

    // Step 5: INSERT (SQL)
    public void SaveScore(string playerName, int score, float completionTime)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        playerName = playerName.Trim();

        // Parameterized SQL 
        _db.Execute(
            "INSERT INTO HighScores (playerName, score, completionTime) VALUES (?, ?, ?);",
            playerName, score, completionTime
        );
    }

    // Step 6: SELECT top N ordered (SQL)
    public List<HighScoreEntry> GetTopHighScores(int limit = 5)
    {
        if (limit < 0) limit = 0;

        // Query maps returned columns into HighScoreEntry fields (by name)
        return _db.Query<HighScoreEntry>(
            @"SELECT id, playerName, score, completionTime
              FROM HighScores
              ORDER BY score DESC, completionTime ASC
              LIMIT ?;",
            limit
        );
    }

    public void ClearAllScores()
    {
        _db.Execute("DELETE FROM HighScores;");
        try { _db.Execute("VACUUM;"); } catch { }
    }
}