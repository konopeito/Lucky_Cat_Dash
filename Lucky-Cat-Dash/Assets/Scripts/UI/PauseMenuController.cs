using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;

    [Header("Volume")]
    public Slider volumeSlider;
    public TMP_Text volumeLabel;

    private bool isPaused;

    private void Start()
    {
        SaveData data = SaveSystem.Load();
        if (volumeSlider != null)
        {
            volumeSlider.value = data.masterVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            ApplyVolume(data.masterVolume);
        }

        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.QuitToMenu();
    }

    private void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        SaveSystem.SaveVolume(value);
        if (volumeLabel != null)
            volumeLabel.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
    }

    private void ApplyVolume(float value)
    {
        AudioListener.volume = value;
    }
}
