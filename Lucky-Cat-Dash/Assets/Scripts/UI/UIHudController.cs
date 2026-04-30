using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHudController : MonoBehaviour
{
    public Slider luckSlider;
    public TMP_Text charmText;
    public TMP_Text timerText;

    [Header("Coins (Yen) UI")]
    public TMP_Text yenText;

    [Header("Player Ref")]
    public LuckSystem playerLuck;

    private void Start()
    {
        // Luck meter updates (delegate/event)
        if (playerLuck != null)
        {
            playerLuck.OnLuckChanged += HandleLuckChanged;
            HandleLuckChanged(playerLuck.CurrentLuck, playerLuck.MaxLuck);
        }

        // Score counters (events from GameManager)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCharmCountChanged += HandleCharmChanged;
            GameManager.Instance.OnYenChanged += HandleYenChanged;

            // Initialize UI from current values (in case HUD loads mid-match)
            HandleCharmChanged(GameManager.Instance.charmsCollectedThisMatch);
            HandleYenChanged(GameManager.Instance.yenCollectedThisMatch);
        }
        else
        {
            HandleCharmChanged(0);
            HandleYenChanged(0);
        }
    }

    private void OnDestroy()
    {
        if (playerLuck != null)
            playerLuck.OnLuckChanged -= HandleLuckChanged;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCharmCountChanged -= HandleCharmChanged;
            GameManager.Instance.OnYenChanged -= HandleYenChanged;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.matchActive && timerText != null)
            timerText.text = GameManager.Instance.matchTimer.ToString("F2");
    }

    private void HandleLuckChanged(float current, float max)
    {
        if (luckSlider == null) return;
        luckSlider.maxValue = max;
        luckSlider.value = current;
    }

    private void HandleCharmChanged(int count)
    {
        if (charmText != null)
            charmText.text = $"Charms: {count}";
    }

    private void HandleYenChanged(int yen)
    {
        if (yenText != null)
            yenText.text = $"Yen: {yen}";
    }
}