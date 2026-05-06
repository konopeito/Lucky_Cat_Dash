using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [Tooltip("Each coin is worth 5 yen by default.")]
    public int yenValue = 5;

    [Header("Luck Reward")]
    [Tooltip("How much luck to add when collecting this coin.")]
    public float luckOnCollect = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only let the player collect (prevents other triggers from collecting)
        if (other.GetComponent<PlayerController2D>() == null) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddYen(yenValue);

        // Add luck so the HUD slider updates
        var luck = other.GetComponent<LuckSystem>();
        if (luck != null && luckOnCollect > 0f)
            luck.AddLuck(luckOnCollect);

        gameObject.SetActive(false); // pooled-friendly
    }
}