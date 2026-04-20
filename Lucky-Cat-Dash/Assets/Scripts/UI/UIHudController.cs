using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHudController : MonoBehaviour
{
    public Slider luckSlider;
    public TMP_Text charmText;
    public TMP_Text timerText;

    [Header("Player Ref")]
    public LuckSystem playerLuck;

    private void Start()
    {
        if (playerLuck != null)
        {
            playerLuck.OnLuckChanged += HandleLuckChanged;
            HandleLuckChanged(playerLuck.CurrentLuck, playerLuck.MaxLuck);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnCharmCountChanged += HandleCharmChanged;

        HandleCharmChanged(0);
    }

    private void OnDestroy()
    {
        if (playerLuck != null)
            playerLuck.OnLuckChanged -= HandleLuckChanged;

        if (GameManager.Instance != null)
            GameManager.Instance.OnCharmCountChanged -= HandleCharmChanged;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.matchActive)
            timerText.text = GameManager.Instance.matchTimer.ToString("F2");
    }

    private void HandleLuckChanged(float current, float max)
    {
        luckSlider.maxValue = max;
        luckSlider.value = current;
    }

    private void HandleCharmChanged(int count)
    {
        charmText.text = $"Charms: {count}";
    }
}
