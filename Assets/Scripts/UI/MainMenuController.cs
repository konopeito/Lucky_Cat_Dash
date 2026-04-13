using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Mode Buttons")]
    public Button singlePlayerButton;
    public Button coopButton;
    public Button competitiveButton;
    public Button quitButton;

    [Header("Volume")]
    public Slider volumeSlider;

    private void Start()
    {
        SaveData data = SaveSystem.Load();
        if (volumeSlider != null)
        {
            volumeSlider.value = data.masterVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            AudioListener.volume = data.masterVolume;
        }

        singlePlayerButton.onClick.AddListener(() => StartGame(GameMode.SinglePlayer));
        coopButton.onClick.AddListener(() => StartGame(GameMode.MultiplayerCoop));
        competitiveButton.onClick.AddListener(() => StartGame(GameMode.MultiplayerCompetitive));
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame(GameMode mode)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMatch(mode);
            GameManager.Instance.LoadScene("Level1");
        }
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        SaveSystem.SaveVolume(value);
    }
}
