using System.Collections.Generic;
using UnityEngine;

namespace PowerUps
{
    public class IncreaseGravity : PowerUp
    {
        [SerializeField] private float multiplier = 2.0f;

        protected override void OnPickup(PlayerLevelManager pickupPlayer, IEnumerable<PlayerLevelManager> otherPlayers)
        {
            foreach (var player in otherPlayers)
                player.GetComponent<IncreaseGravityReceiver>().ApplyEffects(multiplier);
        }
    }
}