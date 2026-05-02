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

    [Header("Run UI (optional but recommended)")]
    public TMP_Text livesText;

    [Header("Timer Colors")]
    public Color normalTimerColor = Color.white;
    public Color penaltyTimerColor = Color.red;

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

        //  Timer is now driven by RunTimer (ToriiGate controls start/pause)
        if (RunTimer.Instance != null)
        {
            RunTimer.Instance.OnTimeChanged += HandleTimeChanged;
            RunTimer.Instance.OnPenaltyStateChanged += HandlePenaltyStateChanged;

            HandleTimeChanged(RunTimer.Instance.TimeRemaining);
            HandlePenaltyStateChanged(RunTimer.Instance.IsPenalized);
        }
        else
        {
            // Fallback: keep something visible
            if (timerText != null) timerText.text = "00:00.00";
        }

        //  Lives display from RunManager
        if (RunManager.Instance != null)
        {
            RunManager.Instance.OnLivesChanged += HandleLivesChanged;
            HandleLivesChanged(RunManager.Instance.Lives);
        }
        else
        {
            HandleLivesChanged(0);
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

        if (RunTimer.Instance != null)
        {
            RunTimer.Instance.OnTimeChanged -= HandleTimeChanged;
            RunTimer.Instance.OnPenaltyStateChanged -= HandlePenaltyStateChanged;
        }

        if (RunManager.Instance != null)
            RunManager.Instance.OnLivesChanged -= HandleLivesChanged;
    }

    // no longer need Update() for the timer; RunTimer events drive it.

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

    private void HandleLivesChanged(int lives)
    {
        if (livesText != null)
            livesText.text = $"Lives: {lives}";
    }

    private void HandleTimeChanged(float timeRemaining)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        float seconds = timeRemaining % 60f;
        timerText.text = $"{minutes:00}:{seconds:00.00}";
    }

    private void HandlePenaltyStateChanged(bool penalized)
    {
        if (timerText == null) return;
        timerText.color = penalized ? penaltyTimerColor : normalTimerColor;
    }
}