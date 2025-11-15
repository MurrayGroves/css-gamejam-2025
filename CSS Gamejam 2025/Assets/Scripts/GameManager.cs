using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PlayerLevelManager> allPlayers;

    private void Awake()
    {
        allPlayers = new();
    }

    public void RegisterPlayerLevelManager(PlayerLevelManager playerLevelManager)
    {
        allPlayers.Add(playerLevelManager);
    }
}