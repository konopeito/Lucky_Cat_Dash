using System.Collections;
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

    [Header("Run UI (optional)")]
    public TMP_Text livesText;

    [Header("Timer Colors")]
    public Color normalTimerColor = Color.white;
    public Color penaltyTimerColor = Color.red;

    [Header("Player Ref")]
    public LuckSystem playerLuck;

    [Header("Ability Icons")]
    public Image doubleJumpIcon;
    public Image dashIcon;
    public Image slowFallIcon;

    [Tooltip("Color when ability is usable.")]
    public Color abilityReadyColor = Color.white;

    [Tooltip("Color when ability is locked/unusable.")]
    public Color abilityLockedColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Ability Use Feedback (Pulse)")]
    [Tooltip("How big the icon scales during the pulse.")]
    public float pulseScale = 1.25f;

    [Tooltip("How long each half of the pulse takes (up then down).")]
    public float pulseDuration = 0.12f;

    [Header("Slow Fall Active Feedback (Glow)")]
    [Tooltip("Optional color used while slow fall is actively draining luck. Leave as white if you just want pulse.")]
    public Color slowFallActiveColor = new Color(0.6f, 0.9f, 1f, 1f);

    private PlayerAbilities _abilities;
    private PlayerController2D _playerController;

    // track base scale so pulses don't permanently distort UI
    private Vector3 _doubleJumpBaseScale = Vector3.one;
    private Vector3 _dashBaseScale = Vector3.one;
    private Vector3 _slowFallBaseScale = Vector3.one;

    private Coroutine _doubleJumpPulseRoutine;
    private Coroutine _dashPulseRoutine;
    private Coroutine _slowFallPulseRoutine;

    private void Start()
    {
        // Cache base scales
        if (doubleJumpIcon != null) _doubleJumpBaseScale = doubleJumpIcon.rectTransform.localScale;
        if (dashIcon != null) _dashBaseScale = dashIcon.rectTransform.localScale;
        if (slowFallIcon != null) _slowFallBaseScale = slowFallIcon.rectTransform.localScale;

        // Find abilities + controller from the same player object as LuckSystem
        if (playerLuck != null)
        {
            _abilities = playerLuck.GetComponent<PlayerAbilities>();
            _playerController = playerLuck.GetComponent<PlayerController2D>();
        }

        // Luck meter updates (delegate/event)
        if (playerLuck != null)
        {
            playerLuck.OnLuckChanged += HandleLuckChanged;

            // initialize immediately
            HandleLuckChanged(playerLuck.CurrentLuck, playerLuck.MaxLuck);
            UpdateAbilityIcons();
        }
        else
        {
            // ensure icons don't show as ready when no player ref is assigned
            UpdateAbilityIcons();
        }

        // Subscribe to ability-use events (for pulsing/glow)
        if (_abilities != null)
        {
            _abilities.OnDashUsed += HandleDashUsed;
            _abilities.OnSlowFallTick += HandleSlowFallTick;
            _abilities.OnSlowFallActiveChanged += HandleSlowFallActiveChanged;
        }

        if (_playerController != null)
        {
            _playerController.OnDoubleJumpUsed += HandleDoubleJumpUsed;
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

        // Timer is now driven by RunTimer (ToriiGate controls start/pause)
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

        // Lives display from RunManager
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

        if (_abilities != null)
        {
            _abilities.OnDashUsed -= HandleDashUsed;
            _abilities.OnSlowFallTick -= HandleSlowFallTick;
            _abilities.OnSlowFallActiveChanged -= HandleSlowFallActiveChanged;
        }

        if (_playerController != null)
            _playerController.OnDoubleJumpUsed -= HandleDoubleJumpUsed;

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

    private void HandleLuckChanged(float current, float max)
    {
        if (luckSlider != null)
        {
            luckSlider.maxValue = max;
            luckSlider.value = current;
        }

        // refresh ability icons any time luck changes
        UpdateAbilityIcons();
    }

    private void UpdateAbilityIcons()
    {
        if (playerLuck == null)
        {
            // No player ref → show as locked
            SetIcon(doubleJumpIcon, false);
            SetIcon(dashIcon, false);
            SetIcon(slowFallIcon, false);
            return;
        }

        SetIcon(doubleJumpIcon, playerLuck.CanDoubleJump);
        SetIcon(dashIcon, playerLuck.CanDash);
        SetIcon(slowFallIcon, playerLuck.CanSlowFall);
    }

    private void SetIcon(Image icon, bool ready)
    {
        if (icon == null) return;

        // Note: slow-fall active glow will override this color while active.
        icon.color = ready ? abilityReadyColor : abilityLockedColor;
    }

    // -----------------------
    // Ability use feedback
    // -----------------------
    private void HandleDoubleJumpUsed()
    {
        Pulse(doubleJumpIcon, ref _doubleJumpPulseRoutine, _doubleJumpBaseScale);
    }

    private void HandleDashUsed()
    {
        Pulse(dashIcon, ref _dashPulseRoutine, _dashBaseScale);
    }

    private void HandleSlowFallTick()
    {
        // Slow fall can tick every FixedUpdate; keep pulse subtle/quick.
        Pulse(slowFallIcon, ref _slowFallPulseRoutine, _slowFallBaseScale);
    }

    private void HandleSlowFallActiveChanged(bool active)
    {
        if (slowFallIcon == null) return;

        if (!active)
        {
            // when slow-fall stops, revert icon color to ready/locked state based on current luck
            UpdateAbilityIcons();
            return;
        }

        // when slow-fall is active, apply a glow color (but only if slow fall is actually available)
        if (playerLuck != null && playerLuck.CanSlowFall)
            slowFallIcon.color = slowFallActiveColor;
    }

    private void Pulse(Image icon, ref Coroutine routine, Vector3 baseScale)
    {
        if (icon == null) return;

        // restart pulse if already running
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(PulseRoutine(icon.rectTransform, baseScale));
    }

    private IEnumerator PulseRoutine(RectTransform rt, Vector3 baseScale)
    {
        if (rt == null) yield break;

        Vector3 target = baseScale * pulseScale;

        float t = 0f;
        while (t < pulseDuration)
        {
            t += Time.unscaledDeltaTime; // works even while paused
            float a = t / pulseDuration;
            rt.localScale = Vector3.Lerp(baseScale, target, a);
            yield return null;
        }

        t = 0f;
        while (t < pulseDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = t / pulseDuration;
            rt.localScale = Vector3.Lerp(target, baseScale, a);
            yield return null;
        }

        rt.localScale = baseScale;
    }

    // -----------------------
    // Existing HUD updates
    // -----------------------
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