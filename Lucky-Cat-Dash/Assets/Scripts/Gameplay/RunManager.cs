using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Lives")]
    public int startingLives = 3;
    public int maxLives = 4;

    public int Lives { get; private set; }

    [Header("Hazard Penalty")]
    public float hazardTimePenaltySeconds = 5f;

    [Header("Charm Rules")]
    [Tooltip("How many charms the player drops on hazard hit (0 = drop all).")]
    public int dropCharmCountOnHit = 0;

    [Tooltip("Prefab to spawn when dropping a charm.")]
    public GameObject droppedCharmPrefab;

    [Tooltip("How far to scatter dropped charms around the player.")]
    public float dropScatterRadius = 0.8f;

    [Header("References (assign in scene)")]
    public PlayerController2D player;
    public LuckSystem playerLuck;

    // “This run” inventory separate from SaveSystem’s total.
    public int CharmsHeldThisRun { get; private set; }

    // Torii “stamps”
    [Serializable]
    public struct Stamp
    {
        public int stampIndex;
        public int charmsAtStamp;
        public int yenAtStamp;
        public float timeRemainingAtStamp;
    }

    public event Action<int> OnLivesChanged;
    public event Action<int> OnCharmsHeldChanged;
    public event Action<Stamp> OnStampCreated;

    private int _stampIndex;
    private int _stampsCreated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartNewRun();
    }

    public void StartNewRun()
    {
        Lives = startingLives;
        CharmsHeldThisRun = 0;
        _stampIndex = 0;
        _stampsCreated = 0;

        OnLivesChanged?.Invoke(Lives);
        OnCharmsHeldChanged?.Invoke(CharmsHeldThisRun);

        RunTimer.Instance?.ResetRun();
    }

    public void AddCharmHeld(int amount)
    {
        if (amount == 0) return;
        CharmsHeldThisRun = Mathf.Max(0, CharmsHeldThisRun + amount);
        OnCharmsHeldChanged?.Invoke(CharmsHeldThisRun);
    }

    public void ConsumeLife()
    {
        Lives = Mathf.Max(0, Lives - 1);
        OnLivesChanged?.Invoke(Lives);

        if (Lives <= 0)
            FailRun();
    }

    public void GrantLifeOnce(int amount = 1)
    {
        Lives = Mathf.Min(maxLives, Lives + Mathf.Max(0, amount));
        OnLivesChanged?.Invoke(Lives);
    }

    public void OnHazardHit(Vector3 playerPosition)
    {
        // 1) Reset luck multiplier
        if (playerLuck != null)
            playerLuck.ResetLuckMultiplier();

        // 2) Drop charms held this run
        DropCharms(playerPosition);

        // 3) Time penalty (do NOT reset timer)
        RunTimer.Instance?.SubtractTime(hazardTimePenaltySeconds, markPenalty: true);

        // 4) Lose a life (this can trigger FailRun)
        ConsumeLife();

        // 5) Respawn (only if still alive)
        if (Lives > 0 && player != null)
            player.Respawn();
    }

    private void DropCharms(Vector3 origin)
    {
        if (CharmsHeldThisRun <= 0) return;

        // If prefab missing, just delete held charms so run continues without errors
        if (droppedCharmPrefab == null)
        {
            CharmsHeldThisRun = 0;
            OnCharmsHeldChanged?.Invoke(CharmsHeldThisRun);
            return;
        }

        int dropCount = dropCharmCountOnHit <= 0
            ? CharmsHeldThisRun
            : Mathf.Min(CharmsHeldThisRun, dropCharmCountOnHit);

        for (int i = 0; i < dropCount; i++)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle * dropScatterRadius;
            Vector3 pos = origin + new Vector3(offset.x, offset.y, 0f);
            Instantiate(droppedCharmPrefab, pos, Quaternion.identity);
        }

        CharmsHeldThisRun -= dropCount;
        OnCharmsHeldChanged?.Invoke(CharmsHeldThisRun);
    }

    public void CreateStamp()
    {
        _stampIndex++;
        _stampsCreated++;

        int yen = GameManager.Instance != null ? GameManager.Instance.yenCollectedThisMatch : 0;
        float timeRem = RunTimer.Instance != null ? RunTimer.Instance.TimeRemaining : 0f;

        Stamp s = new Stamp
        {
            stampIndex = _stampIndex,
            charmsAtStamp = CharmsHeldThisRun,
            yenAtStamp = yen,
            timeRemainingAtStamp = timeRem
        };

        OnStampCreated?.Invoke(s);

        Debug.Log($"[RunManager] Stamp {_stampIndex}: charms={s.charmsAtStamp}, yen={s.yenAtStamp}, timeRemaining={s.timeRemainingAtStamp:F1}");
    }

    public void OnTimeExpired()
    {
        FailRun();
    }

    private void FailRun()
    {
        Debug.Log("[RunManager] Run failed (permadeath or time expired).");

        // Compute completion time + score HERE 
        float completionTimeSeconds = RunTimer.Instance != null ? RunTimer.Instance.TimeElapsed : 0f;

        int charms = GameManager.Instance != null ? GameManager.Instance.charmsCollectedThisMatch : 0;
        int yen = GameManager.Instance != null ? GameManager.Instance.yenCollectedThisMatch : 0;
        float timeRemaining = RunTimer.Instance != null ? RunTimer.Instance.TimeRemaining : 0f;

        // Simple scoring formula 
        int finalScore =
            charms * 100 +
            yen +
            _stampsCreated * 250 +
            Mathf.RoundToInt(timeRemaining * 2f);

        // Store run result for GameOver UI 
        RunResultCache.Set(
            timeRemainingValue: timeRemaining,
            timeElapsedValue: completionTimeSeconds,
            charmsCollectedValue: charms,
            yenCollectedValue: yen,
            stampsCountValue: _stampsCreated,
            finalScoreValue: finalScore
        );

        // Save to SQLite (player name will be set on GameOver submit; "Player" fallback)
        DatabaseManager.Instance?.SaveScore("Player", finalScore, completionTimeSeconds);

        if (GameManager.Instance != null)
            GameManager.Instance.EndMatch();

     
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}