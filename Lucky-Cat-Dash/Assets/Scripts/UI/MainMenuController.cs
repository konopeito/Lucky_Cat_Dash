using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Mode Buttons")]
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button coopButton;
    [SerializeField] private Button competitiveButton;
    [SerializeField] private Button highScoresButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Optional")]
    [SerializeField] private SceneNav sceneNav;

    [SerializeField] private GameObject highScoresPanel;
    [SerializeField] private HighScoresDisplay highScoresDisplay;



    private void Start()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        if (singlePlayerButton != null) singlePlayerButton.onClick.AddListener(StartSinglePlayer);
        if (coopButton != null) coopButton.onClick.AddListener(StartCoop);
        if (competitiveButton != null) competitiveButton.onClick.AddListener(StartCompetitive);
        if (highScoresButton != null) highScoresButton.onClick.AddListener(OpenHighScoresButton);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettingsButton);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGameButton);
    }

    // --- UI-safe wrappers (show cleanly in OnClick dropdown) ---
    public void StartSinglePlayer() => StartGame(GameMode.SinglePlayer);
    public void StartCoop() => StartGame(GameMode.MultiplayerCoop);
    public void StartCompetitive() => StartGame(GameMode.MultiplayerCompetitive);
    public void OpenHighScoresButton() => OpenHighScoresPanel();
    public void OpenSettingsButton() => OpenSettings();
    public void QuitGameButton() => QuitGame();
    public void CloseSettingsButton() => CloseSettings();

    public void StartGame(GameMode mode)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMatch(mode);
            GameManager.Instance.LoadScene("Level1");
        }
        else
        {
            if (sceneNav != null) sceneNav.LoadLevel1();
            else UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        }
    }
    public void OpenHighScoresPanel()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (highScoresPanel != null) highScoresPanel.SetActive(true);

        if (highScoresDisplay != null) highScoresDisplay.Refresh();
    }

    public void CloseHighScoresPanel()
    {
        if (highScoresPanel != null) highScoresPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }
    //public void OpenHighScores()
    //{
    //    if (GameManager.Instance != null) GameManager.Instance.LoadScene("HighScores");
    //    else if (sceneNav != null) sceneNav.LoadHighScores();
    //    else UnityEngine.SceneManagement.SceneManager.LoadScene("HighScores");
    //}

    public void OpenSettings()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}