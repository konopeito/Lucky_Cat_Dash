using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSaveHandler : MonoBehaviour
{
    [SerializeField] private InputField playerNameInput; // TMP_InputField if using TMP
    [SerializeField] private string nextSceneAfterSubmit = "MainMenu";

    // These should be set from your gameplay manager when game ends
    public int finalScore;
    public float completionTime;

    public void SubmitScore()
    {
        string playerName = playerNameInput != null ? playerNameInput.text : "Player";
        DatabaseManager.Instance.SaveScore(playerName, finalScore, completionTime);

        SceneManager.LoadScene(nextSceneAfterSubmit);
    }

    public void RetryLevel(string levelSceneName)
    {
        string playerName = playerNameInput != null ? playerNameInput.text : "Player";
        DatabaseManager.Instance.SaveScore(playerName, finalScore, completionTime);

        SceneManager.LoadScene(levelSceneName);
    }
}