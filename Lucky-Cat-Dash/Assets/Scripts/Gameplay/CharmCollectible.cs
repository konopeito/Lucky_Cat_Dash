using UnityEngine;

public class CharmCollectible : MonoBehaviour
{
    public float luckValue = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var luck = other.GetComponent<LuckSystem>();
        if (luck == null) return;

        luck.AddLuck(luckValue);

        if (GameManager.Instance != null)
            GameManager.Instance.AddCharm(1);

        gameObject.SetActive(false); // pooled
    }
}
