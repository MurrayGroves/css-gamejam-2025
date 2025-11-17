using System.Collections.Generic;

namespace PowerUps
{
    public class InvertControls : PowerUp
    {
        protected override void OnPickup(PlayerLevelManager pickupPlayer, IEnumerable<PlayerLevelManager> otherPlayers)
        {
            foreach (var player in otherPlayers) player.GetComponent<InvertControlsReceiver>().ApplyEffects();
        }
    }
}