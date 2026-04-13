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

    public event Action<float, float> OnLuckChanged; // current, max

    public float CurrentLuck => currentLuck;
    public float MaxLuck => maxLuck;

    public bool CanDoubleJump => currentLuck >= doubleJumpThreshold;
    public bool CanDash => currentLuck >= dashThreshold;
    public bool CanSlowFall => currentLuck >= slowFallThreshold;

    public void AddLuck(float amount)
    {
        currentLuck = Mathf.Clamp(currentLuck + amount, 0f, maxLuck);
        OnLuckChanged?.Invoke(currentLuck, maxLuck);
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
}
