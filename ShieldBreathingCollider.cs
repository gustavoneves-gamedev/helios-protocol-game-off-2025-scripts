using UnityEngine;

namespace SpaceFusion.SF_Energy_Shield.Scripts {
    [RequireComponent(typeof(SphereCollider))]
    public class ShieldBreathingCollider : MonoBehaviour
    {
        private float _baseRadius = 1f; 
        private float _breatheStrength = 0.2f; 

        private SphereCollider _sphereCollider;

        private void Awake()
        {
            _sphereCollider = GetComponent<SphereCollider>();
            _baseRadius = _sphereCollider.radius;
            _sphereCollider.radius = _baseRadius;
        }
        
        public void SetNewStrength(float strength) {
            _breatheStrength = strength;
        }

        public void UpdateRadius(float phase)
        {
            // Shader logic: sin(time * frequency) * strength
            // var sine = Mathf.Sin(Time.time * _breatheFrequency);
            var sine = Mathf.Sin(phase);
            var offset = sine * _breatheStrength;
            // Apply breathing effect to collider radius
            _sphereCollider.radius = _baseRadius + offset;
        }
    }
}