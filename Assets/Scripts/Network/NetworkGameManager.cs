using System;
using UnityEngine;

/// <summary>
/// Manages multiplayer session state.
/// Extends GameManager logic for host/client authority over collectibles,
/// hazards, and scoring in Co-op and Competitive modes.
/// Full network transport (e.g., Unity Netcode for GameObjects) should be
/// wired up here when the networking package is integrated.
/// </summary>
public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    [Header("Session")]
    public bool isHost;
    public int maxPlayers = 3;
    public int connectedPlayers;

    public event Action<int> OnPlayerConnected;
    public event Action<int> OnPlayerDisconnected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called when a new player joins the session.
    /// </summary>
    public void PlayerConnected(int playerId)
    {
        connectedPlayers++;
        OnPlayerConnected?.Invoke(playerId);
    }

    /// <summary>
    /// Called when a player disconnects. Despawns that player and continues match.
    /// </summary>
    public void PlayerDisconnected(int playerId)
    {
        connectedPlayers = Mathf.Max(0, connectedPlayers - 1);
        OnPlayerDisconnected?.Invoke(playerId);
    }

    /// <summary>
    /// Host-authoritative: notifies all clients that a charm was collected.
    /// In a full implementation this would send an RPC/message over the network.
    /// </summary>
    public void HostCollectCharm(int charmId)
    {
        if (!isHost) return;
        // TODO: broadcast CharmCollected(charmId) to all clients
        if (GameManager.Instance != null)
            GameManager.Instance.AddCharm(1);
    }

    /// <summary>
    /// Determines the winner in Competitive mode based on charms or time.
    /// </summary>
    public void EvaluateCompetitiveResult()
    {
        if (GameManager.Instance == null) return;
        // Placeholder: in a full implementation, gather stats from all peers
        // and decide who won before calling EndMatch.
        GameManager.Instance.EndMatch();
        GameManager.Instance.LoadScene("Results");
    }

    /// <summary>
    /// Triggers a Co-op team luck bonus when all connected players
    /// reach the high-luck threshold simultaneously.
    /// </summary>
    public void TriggerTeamLuckBonus()
    {
        // TODO: spawn temporary helper platforms or disable hazards for
        // a short duration as a co-op reward.
        Debug.Log("[NetworkGameManager] Team Luck Bonus triggered!");
    }
}
