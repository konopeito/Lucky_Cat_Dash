using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [Tooltip("Each coin is worth 5 yen by default.")]
    public int yenValue = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only let the player collect (prevents other triggers from collecting)
        if (other.GetComponent<PlayerController2D>() == null) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddYen(yenValue);

        // play SFX (requires adding a coin clip + method)
        // if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinCollect();

        gameObject.SetActive(false); // pooled-friendly
    }
}