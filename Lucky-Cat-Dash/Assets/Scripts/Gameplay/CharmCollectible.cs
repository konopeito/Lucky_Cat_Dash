using UnityEngine;

public class CharmCollectible : MonoBehaviour
{
    public float luckValue = 10f;

    [Header("Run System")]
    [Tooltip("If true and RunManager exists, also track charms held this run (for drops/time bonuses).")]
    [SerializeField] private bool trackHeldCharmThisRun = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only the player should collect
        if (other.GetComponent<PlayerController2D>() == null) return;

        var luck = other.GetComponent<LuckSystem>();
        if (luck == null) return;

        // Add luck (will be multiplied if LuckSystem has the multiplier logic)
        luck.AddLuck(luckValue);

        // Match score
        if (GameManager.Instance != null)
            GameManager.Instance.AddCharm(1);

        // Run inventory (used for hazard drops + shrine bonuses)
        if (trackHeldCharmThisRun && RunManager.Instance != null)
            RunManager.Instance.AddCharmHeld(1);

        gameObject.SetActive(false); // pooled-friendly
    }
}