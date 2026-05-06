using System;
using UnityEngine;

public class RunTimer : MonoBehaviour
{
    public static RunTimer Instance { get; private set; }

    [Header("Timer")]
    [Tooltip("Starting time for a run (seconds).")]
    public float startTimeSeconds = 120f;

    [Tooltip("If true, timer will not start until ToriiGate triggers it.")]
    public bool startPaused = true;

    [Header("Penalty UI State")]
    [Tooltip("How long (seconds) the timer is considered 'penalized' after a hazard hit.")]
    public float penaltyStateDuration = 2f;

    public bool IsRunning { get; private set; }
    public bool IsPenalized => _penaltyUntilTime > Time.unscaledTime;

    public float TimeRemaining => _timeRemaining;
    public float TimeElapsed => startTimeSeconds - _timeRemaining;

    public event Action<float> OnTimeChanged;              // timeRemaining
    public event Action<bool> OnRunningChanged;            // running?
    public event Action<bool> OnPenaltyStateChanged;       // penalized?

    private float _timeRemaining;
    private float _penaltyUntilTime = -999f;
    private bool _lastPenaltyState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //** reminder - For now I will keep per-level (DON'T DontDestroyOnLoad unless I rebind UI/player each scene)
    }
    public void SetPaused(bool paused)
    {
        if (paused) Pause();
        else Resume();
    }
    private void Start()
    {
        ResetRun();
        if (!startPaused) Resume();
        else Pause();
    }

    private void Update()
    {
        if (!IsRunning) { UpdatePenaltyEvent(); return; }

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining < 0f) _timeRemaining = 0f;

        OnTimeChanged?.Invoke(_timeRemaining);

        if (_timeRemaining <= 0f)
        {
            Pause();
            RunManager.Instance?.OnTimeExpired();
        }

        UpdatePenaltyEvent();
    }

    private void UpdatePenaltyEvent()
    {
        bool now = IsPenalized;
        if (now != _lastPenaltyState)
        {
            _lastPenaltyState = now;
            OnPenaltyStateChanged?.Invoke(now);
        }
    }

    public void ResetRun()
    {
        _timeRemaining = Mathf.Max(0f, startTimeSeconds);
        OnTimeChanged?.Invoke(_timeRemaining);
        _penaltyUntilTime = -999f;
        _lastPenaltyState = false;
        OnPenaltyStateChanged?.Invoke(false);
    }

    public void Resume()
    {
        if (IsRunning) return;
        IsRunning = true;
        OnRunningChanged?.Invoke(true);
    }

    public void Pause()
    {
        if (!IsRunning) return;
        IsRunning = false;
        OnRunningChanged?.Invoke(false);
    }

    public void Toggle()
    {
        if (IsRunning) Pause();
        else Resume();
    }

    public void AddTime(float seconds)
    {
        if (seconds <= 0f) return;
        _timeRemaining += seconds;
        OnTimeChanged?.Invoke(_timeRemaining);
    }

    public void SubtractTime(float seconds, bool markPenalty = true)
    {
        if (seconds <= 0f) return;
        _timeRemaining = Mathf.Max(0f, _timeRemaining - seconds);
        OnTimeChanged?.Invoke(_timeRemaining);

        if (markPenalty)
            _penaltyUntilTime = Time.unscaledTime + penaltyStateDuration;
    }
}