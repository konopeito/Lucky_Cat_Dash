using UnityEngine;

/// <summary>
/// Placed at the shrine/goal of each level.
/// Ends the match and transitions to the Results scene when a player reaches it.
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController2D>() == null) return;

        if (GameManager.Instance == null) return;

        GameManager.Instance.EndMatch();
        GameManager.Instance.LoadScene("Results");
    }
}
