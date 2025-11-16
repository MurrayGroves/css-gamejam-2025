using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public readonly List<PlayerLevelManager> allPlayers = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RegisterPlayerLevelManager(PlayerLevelManager playerLevelManager)
    {
        allPlayers.Add(playerLevelManager);
    }
}