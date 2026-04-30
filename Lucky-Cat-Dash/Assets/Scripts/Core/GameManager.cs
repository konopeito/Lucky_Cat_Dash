using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    SinglePlayer,
    MultiplayerCoop,
    MultiplayerCompetitive
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameMode currentMode = GameMode.SinglePlayer;
    public bool matchActive;
    public float matchTimer;

    [Header("Scoring")]
    public int charmsCollectedThisMatch;

    public event Action OnMatchStarted;
    public event Action OnMatchEnded;
    public event Action<int> OnCharmCountChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (matchActive) matchTimer += Time.deltaTime;
    }

    public void StartMatch(GameMode mode)
    {
        currentMode = mode;
        matchActive = true;
        matchTimer = 0f;
        charmsCollectedThisMatch = 0;
        OnMatchStarted?.Invoke();
    }

    public void EndMatch()
    {
        matchActive = false;
        SaveSystem.SaveMatchResult(matchTimer, charmsCollectedThisMatch);
        OnMatchEnded?.Invoke();
    }

    public void AddCharm(int amount = 1)
    {
        charmsCollectedThisMatch += amount;
        OnCharmCountChanged?.Invoke(charmsCollectedThisMatch);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitToMenu()
    {
        matchActive = false;
        SceneManager.LoadScene("MainMenu");
    }
    public int GetFinalScore()
    {
        return charmsCollectedThisMatch;
    }

    public float GetCompletionTime()
    {
        return matchTimer;
    }
}
