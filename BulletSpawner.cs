
using UnityEngine;

namespace SpaceFusion.SF_Energy_Shield.Scripts.Bullets {
    /// <summary>
    /// Given a list of SpawnPositions, the bulletSpawner spawns a bullet on a randomly chosen position from the list.
    /// The Spawn function can be triggered by pressing Space
    /// </summary>
    public class BulletSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject bulletPrefab;
        public Transform[] spawnPositions;

        [Header("Bullet Settings")]
        public Transform target;
        public float bulletSpeed = 10f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBullet();
            }
        }

        void SpawnBullet()
        {
            if (bulletPrefab == null || spawnPositions.Length == 0 || target == null)
            {
                Debug.LogWarning("Missing references on BulletSpawner!");
                return;
            }

            // Pick random spawn position
            var randomSpawn = spawnPositions[Random.Range(0, spawnPositions.Length)];

            // Spawn bullet
            var bulletObj = Instantiate(bulletPrefab, randomSpawn.position, Quaternion.identity);

            // Set bullet target and velocity
            var bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.SetTarget(target, bulletSpeed);
            }
        }
    }
}