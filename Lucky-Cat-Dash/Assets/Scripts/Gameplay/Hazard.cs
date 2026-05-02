using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Luck Penalty")]
    [Tooltip("If true, resets the player's luck gain multiplier when they touch the hazard.")]
    [SerializeField] private bool resetLuckMultiplierOnHit = true;

    [Tooltip("If true, also resets current luck to 0 when they touch the hazard.")]
    [SerializeField] private bool resetCurrentLuckOnHit = false;

    [Header("Run System")]
    [Tooltip("If true and RunManager exists, use it for hazard handling (lives, time penalty, charm drop, respawn).")]
    [SerializeField] private bool useRunManagerIfAvailable = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController2D>();
        if (player == null) return;

        // central system handles everything (time penalty, lives, charm drop, respawn, multiplier reset, etc.)
        if (useRunManagerIfAvailable && RunManager.Instance != null)
        {
            RunManager.Instance.OnHazardHit(player.transform.position);
            return;
        }

        // Fallback (old/simple behavior)
        var luck = other.GetComponent<LuckSystem>();
        if (luck != null)
        {
            if (resetLuckMultiplierOnHit)
                luck.ResetLuckMultiplier();

            if (resetCurrentLuckOnHit)
                luck.ResetLuck();
        }

        player.Respawn();
    }
}