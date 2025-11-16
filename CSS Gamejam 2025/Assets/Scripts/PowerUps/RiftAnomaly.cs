using UnityEngine;
namespace PowerUps
{
    public class RiftAnomaly : MonoBehaviour
    {
        public AudioSource sfxPlayer;
        public AudioClip riftSound;
        public PlayerLevelManager playerLevelManager;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Swapping positions");
            sfxPlayer.clip = riftSound;
            sfxPlayer.Play();
            var playerOne = playerLevelManager.gameManager.allPlayers[0];
            var playerTwo = playerLevelManager.gameManager.allPlayers[1];
            var oldPosPlayerOne = playerOne.transform.position;
            playerOne.Teleport(playerTwo.transform.position);
            playerTwo.Teleport(oldPosPlayerOne);
        }
    }
}