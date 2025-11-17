using System.Collections.Generic;
using UnityEngine;

namespace PowerUps
{
    public class IceStorm : PowerUp
    {
        [SerializeField] private float multiplier = 5.0f;

        protected override void OnPickup(PlayerLevelManager pickupPlayer, IEnumerable<PlayerLevelManager> otherPlayers)
        {
            foreach (var player in otherPlayers) player.GetComponent<IceStormReceiver>().ApplyEffects(multiplier);
        }
    }
}