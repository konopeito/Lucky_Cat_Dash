using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;

    [Tooltip("TMP Input Field where player types their name.")]
    public TMP_InputField nameInput;

    public TMP_Text feedbackText;

    [Header("Scene Names")]
    public string retrySceneName = "Level1";
    public string mainMenuSceneName = "MainMenu";
    public string highScoresSceneName = "HighScores";

    private int _finalScore;
    private float _completionTime;

    private void Start()
    {
        // Pull from  run cache 
        if (RunResultCache.HasResult)
        {
            _finalScore = RunResultCache.finalScore;
            _completionTime = RunResultCache.timeElapsed;
        }
        else
        {
            _finalScore = 0;
            _completionTime = 0f;
        }

        if (scoreText) scoreText.text = $"Score: {_finalScore}";
        if (timeText) timeText.text = $"Time: {FormatTime(_completionTime)}";

        if (nameInput && string.IsNullOrWhiteSpace(nameInput.text))
            nameInput.text = "Player";
    }

    public void SubmitScore()
    {
        string playerName = nameInput != null ? nameInput.text : "Player";

        DatabaseManager.Instance?.SaveScore(playerName, _finalScore, _completionTime);

        if (feedbackText) feedbackText.text = "Saved!";
    }

    public void Retry()
    {
        SubmitScore();
        RunResultCache.Clear();
        SceneManager.LoadScene(retrySceneName);
    }

    public void QuitToMenu()
    {
        SubmitScore();
        RunResultCache.Clear();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ViewHighScores()
    {
        SubmitScore();
        SceneManager.LoadScene(highScoresSceneName);
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        float s = seconds % 60f;
        return $"{m:00}:{s:00.00}";
    }
}