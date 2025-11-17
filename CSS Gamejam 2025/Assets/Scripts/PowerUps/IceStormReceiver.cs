using UnityEngine;

namespace PowerUps
{
    public class IceStormReceiver : PowerUpReceiver
    {
        private float _originalValue;
        private PhysicsMaterial2D _physics;

        protected override float Duration => 5;

        private void Awake()
        {
            _physics = GetComponent<PlayerLevelManager>().MovementController.GetComponent<Rigidbody2D>().sharedMaterial;
        }

        protected override void ResetEffects()
        {
            _physics.friction = _originalValue;
            Debug.Log("Ice storm reset");
        }

        public void ApplyEffects(float multiplier)
        {
            Debug.Log("Ice storm rec");
            if (SetActive()) return;
            Debug.Log("Ice storm set");
            _originalValue = _physics.friction;
            _physics.friction = _originalValue / multiplier;
        }
    }
}