using System.Collections.Generic;
using UnityEngine;

namespace PowerUps
{
    public class StealSpeed : PowerUp
    {
        [SerializeField] private float thiefMultiplier = 3.0f;
        [SerializeField] private float victimMultiplier = 0.5f;

        protected override void OnPickup(PlayerLevelManager pickupPlayer, IEnumerable<PlayerLevelManager> otherPlayers)
        {
            pickupPlayer.GetComponent<StealSpeedReceiver>().ApplyEffects(thiefMultiplier);

            foreach (var player in otherPlayers)
                player.GetComponent<StealSpeedReceiver>().ApplyEffects(victimMultiplier);
        }
    }
}