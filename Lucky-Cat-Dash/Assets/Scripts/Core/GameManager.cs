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

    [Header("Coins (Yen)")]
    [Tooltip("Total yen collected this match (each coin is typically 5 yen).")]
    public int yenCollectedThisMatch;

    [Header("Coin -> Luck Multiplier Bonus")]
    [Tooltip("Every time the player reaches this many yen, their luck gain multiplier increases.")]
    public int yenPerLuckBonus = 20;

    [Tooltip("How much to increase luck gain multiplier each time yenPerLuckBonus is reached (ex: 0.05 = +5%).")]
    public float luckMultiplierIncreasePerBonus = 0.05f;

    [Tooltip("Max allowed luck multiplier.")]
    public float maxLuckMultiplier = 3f;

    [Tooltip("LuckSystem to modify (assign the local player's LuckSystem).")]
    public LuckSystem localPlayerLuck;

    public event Action OnMatchStarted;
    public event Action OnMatchEnded;
    public event Action<int> OnCharmCountChanged;

    // UI/system event
    public event Action<int> OnYenChanged;

    private int nextLuckBonusAtYen;

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
        yenCollectedThisMatch = 0;

        nextLuckBonusAtYen = Mathf.Max(1, yenPerLuckBonus);

        // Reset multiplier each match .
        if (localPlayerLuck != null)
            localPlayerLuck.ResetLuckMultiplier();

        OnCharmCountChanged?.Invoke(charmsCollectedThisMatch);
        OnYenChanged?.Invoke(yenCollectedThisMatch);

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

    /// <summary>
    /// Adds yen and increases luck gain multiplier every time yenPerLuckBonus is reached.
    /// Example: if each coin is 5 yen, then every 4 coins (20 yen) increases the multiplier.
    /// </summary>
    public void AddYen(int amount)
    {
        if (amount <= 0) return;

        yenCollectedThisMatch += amount;
        OnYenChanged?.Invoke(yenCollectedThisMatch);

        // Increase multiplier at 20,40,60... yen (handles large pickups too)
        while (yenCollectedThisMatch >= nextLuckBonusAtYen)
        {
            if (localPlayerLuck != null)
                localPlayerLuck.IncreaseLuckMultiplier(luckMultiplierIncreasePerBonus, maxLuckMultiplier);

            nextLuckBonusAtYen += Mathf.Max(1, yenPerLuckBonus);
        }
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

    public int GetYenThisMatch()
    {
        return yenCollectedThisMatch;
    }
}