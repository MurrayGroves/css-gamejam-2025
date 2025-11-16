using UnityEngine;
namespace PowerUps
{
    public class RiftAnomaly : PowerUp
    {
        
        public AudioClip riftSound;
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            if (!collidedPlayer) return;
            Debug.Log("Swapping positions");
            sfxPlayer.clip = riftSound;
            sfxPlayer.Play();
            var playerOne = Player.gameManager.allPlayers[0];
            var playerTwo = Player.gameManager.allPlayers[1];
            var oldPosPlayerOne = playerOne.transform.position;
            playerOne.Teleport(playerTwo.transform.position);
            playerTwo.Teleport(oldPosPlayerOne);
            Destroy(this);
        }
    }
}