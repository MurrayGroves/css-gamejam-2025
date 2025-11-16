using UnityEngine;
namespace PowerUps
{
    public class RiftAnomaly : PowerUp
    {
        public AudioClip riftSound;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            if (collidedPlayer == null) return;
            Debug.Log("Swapping positions");
            sfxPlayer.clip = riftSound;
            sfxPlayer.Play();
            var playerOne = gameManager.allPlayers[0];
            var playerTwo = gameManager.allPlayers[1];
            var oldPosPlayerOne = playerOne.transform.position;
            playerOne.Teleport(playerTwo.transform.position);
            playerTwo.Teleport(oldPosPlayerOne);
            Destroy(this);
        }
    }
}