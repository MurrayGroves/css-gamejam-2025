using UnityEngine;

namespace Weapons
{
    public class GrenadeLauncher : Projectile
    {
        private static readonly int Explode1 = Animator.StringToHash("Explode");
        private Animator _animator;
        private bool _exploding;
        private Transform _target;

        public void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("GRENADE COLL");
            if (_exploding) return;
            Debug.Log("Didn't return");
            if (other.CompareTag("Player"))
            {
                Debug.Log("Found player");
                _target = other.transform;
                _exploding = true;
                Invoke(nameof(Explode), 0.5f);
            }
        }

        private void Explode()
        {
            Debug.Log("DOing explosion");
            var direction = (Vector2)_target.transform.position - Rb.position;
            var force = 1000.0f * direction * (GetComponent<CircleCollider2D>().radius / direction.magnitude);

            _target.GetComponent<PlayerMovement>().ApplyForce(force);

            GetComponent<Animator>().SetTrigger(Explode1);
            Destroy(gameObject, 0.5f);
        }
    }
}