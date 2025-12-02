using UnityEngine;

namespace SpaceFusion.SF_Energy_Shield.Scripts.Bullets {
    
    /// <summary>
    /// Simulates a bullet that flies into the shield direction and calls the TakeHit shield function on collision
    /// </summary>
    public class Bullet : MonoBehaviour {
        public int damage = 10;
        private Transform _target;
        private float _speed;

        public void SetTarget(Transform newTarget, float newSpeed) {
            _target = newTarget;
            _speed = newSpeed;
        }

        private void Update() {
            if (_target == null) {
                Destroy(gameObject);
                return;
            }

            // Move towards target
            var direction = (_target.position - transform.position).normalized;
            transform.position += direction * (_speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _target.position) < 0.1f) {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            var shield = collision.gameObject.GetComponent<ShieldController>();
            if (shield != null) {
                shield.TakeHit(collision, damage);
            }
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other) {
            Destroy(gameObject);
        }
    }
}