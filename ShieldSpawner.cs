using SF_wip.SF_Energy_Shield.Scriptables;
using UnityEngine;

namespace SpaceFusion.SF_Energy_Shield.Scripts {
    public class ShieldSpawner : MonoBehaviour {
        [Header("Spawn Settings")]
        public GameObject[] shieldPrefabs;
        public Transform spawnPoint;

        private GameObject _currentShield;
        public int shieldHealth;

        private void Update() {
            if (_currentShield != null) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.X)) {
                Spawn();
            }
        }

        private void Spawn() {
            if (shieldPrefabs.Length == 0 || spawnPoint == null) {
                Debug.LogWarning("Missing references on BulletSpawner!");
                return;
            }

            var randomIndex = Random.Range(0, shieldPrefabs.Length);
            var prefab = shieldPrefabs[randomIndex];

            _currentShield = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            _currentShield.GetComponent<ShieldController>().SetHealth(shieldHealth);
        }
    }
}