using System.Linq;
using UnityEngine;

namespace PowerUps
{
    public class InvertControls : PowerUp
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (Player.gameManager.allPlayers.Count <= 0) return;
            foreach (var player in Player.gameManager.allPlayers.Where(player => player != Player))
            {
                player.InvertControls();
            }
        }
    }
}