using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("Main pause panel (Resume/Settings/Quit).")]
    public GameObject pausePanel;

    [Tooltip("Settings panel to show from pause (can be the same prefab used in MainMenu).")]
    public GameObject settingsPanel;

    [Header("Volume (optional legacy UI)")]
    [Tooltip("If you keep a volume slider directly on the pause panel, wire it here. Otherwise leave null.")]
    public Slider volumeSlider;

    [Tooltip("Optional label for the legacy volume slider.")]
    public TMP_Text volumeLabel;

    private bool isPaused;

    private void Start()
    {
        // Start hidden
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (volumeSlider != null)
        {
            SaveData data = SaveSystem.Load();
            volumeSlider.value = data.masterVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            ApplyVolume(data.masterVolume);
            UpdateVolumeLabel(volumeSlider.value);
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (pausePanel == null)
        {
            Debug.LogError("[PauseMenu] pausePanel is NOT assigned in inspector.");
            return;
        }

        // If settings open, close it first
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettingsFromPause();
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null) pausePanel.SetActive(isPaused);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenSettingsFromPause()
    {
        // Keep game paused while in settings
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettingsFromPause()
    {
        // Return to pause menu (still paused)
        isPaused = true;
        Time.timeScale = 0f;

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
            GameManager.Instance.QuitToMenu();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }


    private void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        SaveSystem.SaveVolume(value);
        UpdateVolumeLabel(value);
    }

    private void ApplyVolume(float value)
    {
        AudioListener.volume = value;
    }

    private void UpdateVolumeLabel(float value)
    {
        if (volumeLabel != null)
            volumeLabel.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
    }
}