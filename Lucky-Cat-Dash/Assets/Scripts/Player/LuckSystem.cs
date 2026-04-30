using System;
using UnityEngine;

public class LuckSystem : MonoBehaviour
{
    [Header("Luck")]
    [SerializeField] private float currentLuck = 0f;
    [SerializeField] private float maxLuck = 100f;

    [Header("Thresholds")]
    public float doubleJumpThreshold = 25f;
    public float dashThreshold = 50f;
    public float slowFallThreshold = 75f;

    [Header("Multiplier")]
    [SerializeField] private float luckGainMultiplier = 1f;
    public float LuckGainMultiplier => luckGainMultiplier;

    public event Action<float, float> OnLuckChanged; // current, max

    //will be used for the UI to show multiplier
    public event Action<float> OnLuckMultiplierChanged;

    public float CurrentLuck => currentLuck;
    public float MaxLuck => maxLuck;

    public bool CanDoubleJump => currentLuck >= doubleJumpThreshold;
    public bool CanDash => currentLuck >= dashThreshold;
    public bool CanSlowFall => currentLuck >= slowFallThreshold;

    public void AddLuck(float amount)
    {
        float finalAmount = amount * luckGainMultiplier;
        currentLuck = Mathf.Clamp(currentLuck + finalAmount, 0f, maxLuck);
        OnLuckChanged?.Invoke(currentLuck, maxLuck);
    }

    public void IncreaseLuckMultiplier(float delta, float maxMultiplier = 3f)
    {
        luckGainMultiplier = Mathf.Clamp(luckGainMultiplier + delta, 1f, maxMultiplier);
        OnLuckMultiplierChanged?.Invoke(luckGainMultiplier);
    }

    public bool SpendLuck(float amount)
    {
        if (currentLuck < amount) return false;
        currentLuck -= amount;
        OnLuckChanged?.Invoke(currentLuck, maxLuck);
        return true;
    }

    public void ResetLuck()
    {
        currentLuck = 0f;
        OnLuckChanged?.Invoke(currentLuck, maxLuck);
    }

    public void ResetLuckMultiplier()
    {
        luckGainMultiplier = 1f;
        OnLuckMultiplierChanged?.Invoke(luckGainMultiplier);
    }
}