using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNav : MonoBehaviour
{
    public void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
    public void LoadLevel1() => SceneManager.LoadScene("Level1");
    public void LoadHighScores() => SceneManager.LoadScene("HighScores");
    public void LoadGameOver() => SceneManager.LoadScene("GameOver");
}