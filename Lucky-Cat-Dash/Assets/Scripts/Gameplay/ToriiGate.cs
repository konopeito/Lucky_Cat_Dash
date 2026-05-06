using UnityEngine;

public class ToriiGate : MonoBehaviour
{
    public enum ToriiAction
    {
        ResumeTimer,
        PauseTimer,
        ToggleTimer
    }

    [Header("Timer Action")]
    public ToriiAction action = ToriiAction.ToggleTimer;

    [Header("Stamping")]
    [Tooltip("If > 0, creates a stamp every Nth time ANY torii triggers (global count).")]
    public int stampEvery = 2;

    private static int _toriiPassCount;

    // prevents spam while staying inside trigger
    private bool _playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController2D>() == null) return;
        if (_playerInside) return;
        _playerInside = true;

        _toriiPassCount++;

        if (RunTimer.Instance != null)
        {
            switch (action)
            {
                case ToriiAction.ResumeTimer:
                    RunTimer.Instance.Resume();
                    break;
                case ToriiAction.PauseTimer:
                    RunTimer.Instance.Pause();
                    break;
                case ToriiAction.ToggleTimer:
                    RunTimer.Instance.Toggle();
                    break;
            }
        }

        if (stampEvery > 0 && (_toriiPassCount % stampEvery == 0))
            RunManager.Instance?.CreateStamp();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController2D>() == null) return;
        _playerInside = false;
    }

    public static void ResetToriiCount() => _toriiPassCount = 0;
}