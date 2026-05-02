using UnityEngine;

public class ShrineCheckpoint : MonoBehaviour
{
    [Header("Checkpoint")]
    public Transform respawnPointOverride; // (if null use shrine position)

    [Header("Life Grant")]
    public bool grantsOneLifeOncePerRun = true;

    [Header("Time Bonus")]
    public bool grantTimeBonusIfReachedFast = true;
    public float fastTimeThresholdSeconds = 50f;   // reached within 50s
    public float fastTimeBonusSeconds = 10f;       // +10 sec

    public bool grantTimeBonusIfHoldingCharm = true;
    public float charmTimeBonusSeconds = 5f;

    private bool _lifeGrantedThisRun;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController2D>();
        if (player == null) return;

        // Set respawn point 
        Vector3 rp = respawnPointOverride != null ? respawnPointOverride.position : transform.position;
        player.SetRespawnPoint(rp);

        // Resume timer when reaching a shrine
        RunTimer.Instance?.Resume();

        // Grant +1 life once per run
        if (grantsOneLifeOncePerRun && !_lifeGrantedThisRun)
        {
            RunManager.Instance?.GrantLifeOnce(1);
            _lifeGrantedThisRun = true;
        }

        // Time bonus if reached fast
        if (grantTimeBonusIfReachedFast && RunTimer.Instance != null)
        {
            float elapsed = RunTimer.Instance.TimeElapsed;
            if (elapsed <= fastTimeThresholdSeconds)
                RunTimer.Instance.AddTime(fastTimeBonusSeconds);
        }

        // Time bonus if holding charm(s)
        if (grantTimeBonusIfHoldingCharm && RunManager.Instance != null && RunManager.Instance.CharmsHeldThisRun > 0)
        {
            RunTimer.Instance?.AddTime(charmTimeBonusSeconds);
        }
    }

    public void ResetRunState()
    {
        _lifeGrantedThisRun = false;
    }
}