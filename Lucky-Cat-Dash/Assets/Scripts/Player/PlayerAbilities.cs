using System;
using UnityEngine;

[RequireComponent(typeof(LuckSystem))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAbilities : MonoBehaviour
{
    [Header("Costs")]
    public float dashLuckCost = 15f;
    public float slowFallPerSecondCost = 8f;

    [Header("Dash")]
    public float dashForce = 14f;
    public float dashCooldown = 0.75f;
    private float lastDashTime = -999f;

    // --- Events for UI feedback ---
    // Fired when a dash actually happens (luck spent + cooldown passed)
    public event Action OnDashUsed;

    // Fired when slow-fall actually applies for this FixedUpdate tick (luck spent)
    public event Action OnSlowFallTick;

    // Fired when slow-fall starts / stops (useful to "glow while active")
    public event Action<bool> OnSlowFallActiveChanged;

    private LuckSystem luck;
    private Rigidbody2D rb;

    private bool slowFallActive;

    private void Awake()
    {
        luck = GetComponent<LuckSystem>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TryDash(float inputX)
    {
        if (!luck.CanDash) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        if (!luck.SpendLuck(dashLuckCost)) return;

        float dir = Mathf.Abs(inputX) > 0.01f ? Mathf.Sign(inputX) : transform.localScale.x;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(new Vector2(dir * dashForce, 0f), ForceMode2D.Impulse);

        lastDashTime = Time.time;

        // UI pulse / feedback
        OnDashUsed?.Invoke();
    }

    /// <summary>
    /// Slow fall while holding jump. Returns true if slow fall was applied this tick.
    /// </summary>
    public bool TrySlowFall(bool holdingJump)
    {
        // If input/ability says no, ensure we publish "inactive"
        if (!holdingJump || !luck.CanSlowFall)
        {
            SetSlowFallActive(false);
            return false;
        }

        float cost = slowFallPerSecondCost * Time.fixedDeltaTime;
        if (!luck.SpendLuck(cost))
        {
            SetSlowFallActive(false);
            return false;
        }

        // Apply slow fall effect
        if (rb.velocity.y < -2f)
            rb.velocity = new Vector2(rb.velocity.x, -2f);

        // UI feedback
        SetSlowFallActive(true);
        OnSlowFallTick?.Invoke();

        return true;
    }

    private void SetSlowFallActive(bool active)
    {
        if (slowFallActive == active) return;
        slowFallActive = active;
        OnSlowFallActiveChanged?.Invoke(active);
    }
}