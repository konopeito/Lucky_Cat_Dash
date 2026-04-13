using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreenController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text matchTimeText;
    public TMP_Text charmsText;
    public TMP_Text bestTimeText;
    public TMP_Text totalCharmsText;
    public Button mainMenuButton;
    public Button playAgainButton;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        playAgainButton.onClick.AddListener(PlayAgain);

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        SaveData data = SaveSystem.Load();

        if (GameManager.Instance != null)
        {
            matchTimeText.text = $"Match Time: {GameManager.Instance.matchTimer:F2}s";
            charmsText.text = $"Charms Collected: {GameManager.Instance.charmsCollectedThisMatch}";
        }

        bestTimeText.text = data.bestTime >= 99999f
            ? "Best Time: --"
            : $"Best Time: {data.bestTime:F2}s";

        totalCharmsText.text = $"Total Charms: {data.totalCharms}";
    }

    private void GoToMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadScene("MainMenu");
    }

    private void PlayAgain()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMatch(GameManager.Instance.currentMode);
            GameManager.Instance.LoadScene("Level1");
        }
    }
}
