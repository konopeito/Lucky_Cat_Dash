using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text modeText;
    public Button startButton;
    public Button backButton;

    private void Start()
    {
        if (GameManager.Instance != null)
            modeText.text = $"Mode: {GameManager.Instance.currentMode}";

        startButton.onClick.AddListener(StartMatch);
        backButton.onClick.AddListener(BackToMenu);
    }

    private void StartMatch()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMatch(GameManager.Instance.currentMode);
            GameManager.Instance.LoadScene("Level1");
        }
    }

    private void BackToMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadScene("MainMenu");
    }
}
