using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Luck Penalty")]
    [Tooltip("If true, resets the player's luck gain multiplier when they touch the hazard.")]
    [SerializeField] private bool resetLuckMultiplierOnHit = true;

    [Tooltip("If true, also resets current luck to 0 when they touch the hazard.")]
    [SerializeField] private bool resetCurrentLuckOnHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController2D>();
        if (player == null) return;

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