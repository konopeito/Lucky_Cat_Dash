using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSaveHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private string nextSceneAfterSubmit = "MainMenu";

    [Header("If true, pull score/time from GameManager at submit time")]
    [SerializeField] private bool useGameManagerValues = true;

    [Header("Manual fallback values (used if useGameManagerValues = false)")]
    public int finalScore;
    public float completionTime;

    public void SubmitScore()
    {
        SaveScore();
        SceneManager.LoadScene(nextSceneAfterSubmit);
    }

    public void RetryLevel(string levelSceneName)
    {
        SaveScore();
        SceneManager.LoadScene(levelSceneName);
    }

    public void SubmitAndGoHighScores()
    {
        SaveScore();
        SceneManager.LoadScene("HighScores");
    }

    private void SaveScore()
    {
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("DatabaseManager.Instance is null, cannot save score.");
            return;
        }

        string playerName = playerNameInput != null ? playerNameInput.text : "Player";

        int scoreToSave = finalScore;
        float timeToSave = completionTime;

        if (useGameManagerValues && GameManager.Instance != null)
        {
            scoreToSave = GameManager.Instance.GetFinalScore();
            timeToSave = GameManager.Instance.GetCompletionTime();
        }

        DatabaseManager.Instance.SaveScore(playerName, scoreToSave, timeToSave);
    }
}