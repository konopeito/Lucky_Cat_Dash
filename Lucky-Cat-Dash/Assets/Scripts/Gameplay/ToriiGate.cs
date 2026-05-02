using UnityEngine;

public class ToriiGate : MonoBehaviour
{
    [Header("Behavior")]
    [Tooltip("If true, first time the player passes this gate it will start/resume the timer. Next ToriiGate will pause, then resume, etc.")]
    public bool togglesTimer = true;

    [Tooltip("Every Nth ToriiGate trigger creates a stamp. Set to 2 to stamp every 2nd gate.")]
    public int stampEvery = 2;

    private static int _toriiPassCount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController2D>() == null) return;

        _toriiPassCount++;

        if (togglesTimer && RunTimer.Instance != null)
            RunTimer.Instance.Toggle();

        if (stampEvery > 0 && (_toriiPassCount % stampEvery == 0))
            RunManager.Instance?.CreateStamp();
    }

    //  reset count when scene loads (or on new run)
    public static void ResetToriiCount() => _toriiPassCount = 0;
}