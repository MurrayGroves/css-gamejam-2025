using UnityEngine;

namespace PowerUps
{
    public class IncreaseGravity : PowerUp
    {
        private const int Gravity = 10;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Player.IncreaseGravity(Gravity);
        }
    }
}