using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoresDisplay : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("5 TMP_Text elements (Rank 1..5).")]
    public TMP_Text[] rows;

    [Header("Optional (for Back button)")]
    [SerializeField] private MainMenuController mainMenu;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        List<DatabaseManager.HighScoreEntry> top = DatabaseManager.Instance != null
            ? DatabaseManager.Instance.GetTopHighScores(5)
            : new List<DatabaseManager.HighScoreEntry>();

        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] == null) continue;

            if (i < top.Count)
            {
                var e = top[i];
                rows[i].text = $"{i + 1}. {e.playerName}  |  {e.score} pts  |  {FormatTime(e.completionTime)}";
            }
            else
            {
                rows[i].text = $"{i + 1}. ---";
            }
        }
    }

    public void Back()
    {
        // Panel workflow: just close the panel
        if (mainMenu != null) mainMenu.CloseHighScoresPanel();
        else gameObject.SetActive(false); // fallback
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        float s = seconds % 60f;
        return $"{m:00}:{s:00.00}";
    }
}