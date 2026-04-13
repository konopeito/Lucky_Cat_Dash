using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monitors all connected players' LuckSystems in Co-op mode.
/// When every player simultaneously exceeds the team luck threshold,
/// fires a team bonus event (e.g., temporary platform spawning, hazard disabling).
/// </summary>
public class TeamLuckBonusController : MonoBehaviour
{
    [Header("Bonus Settings")]
    [Tooltip("Luck value each player must reach to trigger the team bonus.")]
    public float teamBonusThreshold = 75f;

    [Tooltip("Duration of the team bonus effect in seconds.")]
    public float bonusDuration = 5f;

    [Header("Optional Effects")]
    public GameObject[] temporaryPlatforms;
    public GameObject[] hazardObjects;

    private readonly List<LuckSystem> playerLuckSystems = new List<LuckSystem>();
    private bool bonusActive;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentMode != GameMode.MultiplayerCoop)
            return;

        if (bonusActive || playerLuckSystems.Count == 0) return;

        bool allPlayersHighLuck = true;
        foreach (var luckSystem in playerLuckSystems)
        {
            if (luckSystem == null || luckSystem.CurrentLuck < teamBonusThreshold)
            {
                allPlayersHighLuck = false;
                break;
            }
        }

        if (allPlayersHighLuck)
            StartCoroutine(ActivateTeamBonus());
    }

    /// <summary>
    /// Register a player's LuckSystem to be monitored.
    /// </summary>
    public void RegisterPlayer(LuckSystem luckSystem)
    {
        if (!playerLuckSystems.Contains(luckSystem))
            playerLuckSystems.Add(luckSystem);
    }

    /// <summary>
    /// Unregister a player's LuckSystem (on disconnect or level unload).
    /// </summary>
    public void UnregisterPlayer(LuckSystem luckSystem)
    {
        playerLuckSystems.Remove(luckSystem);
    }

    private IEnumerator ActivateTeamBonus()
    {
        bonusActive = true;
        Debug.Log("[TeamLuckBonus] Team bonus activated!");

        if (NetworkGameManager.Instance != null)
            NetworkGameManager.Instance.TriggerTeamLuckBonus();

        SetTemporaryPlatforms(true);
        SetHazards(false);

        yield return new WaitForSeconds(bonusDuration);

        SetTemporaryPlatforms(false);
        SetHazards(true);

        bonusActive = false;
    }

    private void SetTemporaryPlatforms(bool active)
    {
        foreach (var platform in temporaryPlatforms)
            if (platform != null) platform.SetActive(active);
    }

    private void SetHazards(bool active)
    {
        foreach (var hazard in hazardObjects)
            if (hazard != null) hazard.SetActive(active);
    }
}
