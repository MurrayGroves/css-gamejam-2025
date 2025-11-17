using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PowerUps
{
    public class RiftAnomaly : PowerUp
    {
        public AudioClip riftSound;

        public new void Start()
        {
            Destroy(gameObject); // disable
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            if (collidedPlayer == null) return;
        }

        protected override void OnPickup(PlayerLevelManager pickupPlayer, IEnumerable<PlayerLevelManager> otherPlayers)
        {
            Debug.Log("Swapping positions");
            sfxPlayer.clip = riftSound;
            sfxPlayer.Play();
            var playerOne = pickupPlayer;
            var playerTwo = otherPlayers.First();
            var oldPosPlayerOne = playerOne.transform.position;
            playerOne.Teleport(playerTwo.transform.position);
            playerTwo.Teleport(oldPosPlayerOne);
            Notify(pickupPlayer);
        }
    }
}