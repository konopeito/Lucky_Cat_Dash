using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoresDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text[] scoreLines; // assign 5 rows in Inspector

    public void Refresh()
    {
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("DatabaseManager.Instance is null. Ensure __App exists in scene.");
            return;
        }

        List<DatabaseManager.HighScoreEntry> top = DatabaseManager.Instance.GetTopHighScores(5);

        for (int i = 0; i < scoreLines.Length; i++)
        {
            if (i < top.Count)
            {
                var e = top[i];
                scoreLines[i].text = $"{i + 1}. {e.playerName} | {e.score} | {e.completionTime:F2}s";
            }
            else
            {
                scoreLines[i].text = $"{i + 1}. ---";
            }
        }
    }

    private void OnEnable()
    {
        Refresh();
    }
}