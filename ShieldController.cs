using System;
using SF_wip.SF_Energy_Shield.Scriptables;
using UnityEngine;
using UnityEngine.VFX;

namespace SpaceFusion.SF_Energy_Shield.Scripts {
    /// <summary>
    /// Shield Animation script controlling spawn effect, explosion effects and damage based effects
    /// needs a ShieldBehaviorConfig to extract the needed animationCurves
    /// </summary>
    [RequireComponent(typeof(ShieldBreathingCollider))]
    [RequireComponent(typeof(CopyMaterialProperties))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ShieldController : MonoBehaviour {
        private static readonly int HitStrength = Shader.PropertyToID("_Hit_Strength");
        private static readonly int HitPosition = Shader.PropertyToID("_Hit_Position");
        private static readonly int Visibility = Shader.PropertyToID("_Visibility");
        private static readonly int SphereCenter = Shader.PropertyToID("_Sphere_Center");
        private static readonly int BreatheStrength = Shader.PropertyToID("_Breathe_Strength");
        private static readonly int BreatheFrequency = Shader.PropertyToID("_Breathe_Frequency");
        private static readonly int Phase = Shader.PropertyToID("_Phase");

        public ShieldBehaviorConfig config;

        [Header("Required additional components")]
        private ShieldBreathingCollider _shieldBreathingCollider;
        private CopyMaterialProperties _propCloner;

        [Header("Private references")]
        private VisualEffect _shieldHitVFX;
        private Material _shieldMaterial;
        private int _currentHealth = 100;
        private int _maxHealth = 100;

        // for fixing breathing
        private float _phase;
        // for final hit animation
        private float _currentDuration;
        private Vector3 _entryHitPoint, _exitHitPoint;
        private AnimationState _currentState = AnimationState.Idle;
        private float _sphereDefaultRadius;

        
        /// <summary>
        /// default initialization of all needed properties
        /// and triggers spawn animation
        /// </summary>
        private void Awake() {
            _shieldMaterial = gameObject.GetComponent<Renderer>().material;
            _shieldMaterial.SetFloat(Visibility, 0);
            if (_shieldMaterial == null) {
                throw new Exception("Could not extract material for shield controller. Please add it to the shield gameObject.");
            }

            _sphereDefaultRadius = GetComponent<SphereCollider>().radius;
            _propCloner = GetComponent<CopyMaterialProperties>();
            _shieldBreathingCollider = GetComponent<ShieldBreathingCollider>();

            // set the desired local transform size
            var size = config.shieldSize;
            gameObject.transform.localScale = new Vector3(size, size, size);
            InitializeProperties();
            // set the proper breathe strength to the collider resizer so we have proper functioning colliders
            _shieldBreathingCollider.SetNewStrength(_shieldMaterial.GetVector(BreatheStrength).x);

            // we first want to play the spawn animation
            _currentState = AnimationState.Spawning;

        }

        /// <summary>
        /// Handles the incoming hit.
        /// - Reduces the shield health
        /// - instantiates the hit effect and copies the needed material properties to the instantiated hit material
        /// - Triggers destroying the effect after 1 second (usually you don't want to have the effect for too long, just a quick hit flash)
        /// - Starts final hit if animation state idle and health below zero
        /// </summary>
        public void TakeHit(Collision collision, int damage = 5) {
            if (_currentState is AnimationState.FinalHit or AnimationState.Explosion) {
                // if we already animate the final hit and explosion state we do not need to create hit effects anymore...
                // of course this is dependent on the developer preference and can be removed. I just think it looks more clean when disabled on final animation
                return;
            }

            var hitEffect = Instantiate(config.shieldHitPrefab, transform);
            var hitMat = hitEffect.GetComponent<Renderer>().material;
            _propCloner.CopyProperties(_shieldMaterial, hitMat);
            TakeDamage(damage);


            // VFX animation call
            var particleMat = hitEffect.GetComponent<ParticleSystemRenderer>()?.material;
            if (particleMat) {
                particleMat.SetVector(SphereCenter, collision.contacts[0].point);
            }


            Destroy(hitEffect, 1);

            // shield animation state
            if (_currentHealth <= 0 && _currentState == AnimationState.Idle) {
                var localHit = transform.InverseTransformPoint(collision.contacts[0].point);
                _shieldMaterial.SetVector(HitPosition, localHit);
                StartFinalHit(localHit);
            }
        }

        /// <summary>
        /// Can be used to inject the max health & currentHealth from outside based on player level
        /// Overwrites the maxHealth value from the ShieldBehaviorConfig
        /// </summary>
        public void SetHealth(int value) {
            _maxHealth = value;
            _currentHealth = value;
        }


        /// <summary>
        /// Trigger effect animation for the final hit, starting from the hit position
        /// </summary>
        private void StartFinalHit(Vector3 localHitPos) {
            _entryHitPoint = localHitPos;
            // we take the double radius to calculate the exit point since we want the effect to go fully through the sphere and not stop at the sphere end
            _exitHitPoint = -localHitPos.normalized * _sphereDefaultRadius * 2;
            _currentDuration = 0f;
            _currentState = AnimationState.FinalHit;
        }

        private void Update() {
            var damageTaken = (float)(_maxHealth - _currentHealth) / _maxHealth;
            // always update target values based on the total damage taken
            UpdateTargets(damageTaken);

            // to avoid using unity`s timer shaderGraph node and to keep the hit effect, shield and collider resizer in sync
            _phase += Time.deltaTime * _shieldMaterial.GetFloat(BreatheFrequency) * Mathf.PI * 2f;
            // Keep phase from growing too large
            if (_phase > Mathf.PI * 2f) {
                _phase -= Mathf.PI * 2f;
            }

            _shieldMaterial.SetFloat(Phase, _phase);
            _shieldBreathingCollider.UpdateRadius(_phase);


            foreach (var prop in config.shieldProperties) {
                if (Mathf.Abs(prop.currentValue - prop.targetValue) < 0.001f) {
                    // check to save us unnecessary material property update calls when the values didn't change...
                    continue;
                }

                prop.currentValue = Mathf.Lerp(prop.currentValue, prop.targetValue, Time.deltaTime / config.smoothTime);
                // if close enough to target, then switch to target value
                if (Mathf.Abs(prop.currentValue - prop.targetValue) < 0.001f) {
                    prop.currentValue = prop.targetValue;
                }

                ApplyProperty(prop);
            }

            switch (_currentState) {
                case AnimationState.Idle:
                    return;
                case AnimationState.Spawning: {
                    _currentDuration += Time.deltaTime;
                    var t = Mathf.Clamp01(_currentDuration / config.explodeDuration);

                    // alpha along curve
                    var visibility = config.initVisibilityCurve.Evaluate(t);
                    _shieldMaterial.SetFloat(Visibility, visibility);

                    // Strength along curve
                    var size = config.initSizeCurve.Evaluate(t);
                    _shieldMaterial.SetFloat(HitStrength, size);
                    if (t >= 1f) {
                        _currentState = AnimationState.Idle;
                        _currentDuration = 0f; // so we can reuse timer in Explosion state
                    }

                    return;
                }
                case AnimationState.FinalHit: {
                    _currentDuration += Time.deltaTime;
                    var t = Mathf.Clamp01(_currentDuration / config.travelDuration);

                    // Position along curve
                    var throughPos = Vector3.Lerp(_entryHitPoint, _exitHitPoint, config.positionCurve.Evaluate(t));
                    _shieldMaterial.SetVector(HitPosition, throughPos);

                    // Strength along curve
                    var strength = config.strengthCurve.Evaluate(t);
                    _shieldMaterial.SetFloat(HitStrength, strength);

                    if (t >= 1f) {
                        _currentState = AnimationState.Explosion;
                        _currentDuration = 0f; // so we can reuse timer in Explosion state
                        _shieldMaterial.SetFloat(HitStrength, 0);
                        _shieldMaterial.SetVector(HitPosition, Vector3.zero);
                    }

                    return;
                }
                case AnimationState.Explosion: {
                    _currentDuration += Time.deltaTime;
                    var t = Mathf.Clamp01(_currentDuration / config.explodeDuration);

                    // alpha along curve
                    var visibility = config.visibilityCurve.Evaluate(t);
                    _shieldMaterial.SetFloat(Visibility, visibility);

                    // Strength along curve
                    var strength = config.hitStrengthCurve.Evaluate(t);
                    _shieldMaterial.SetFloat(HitStrength, strength);
                    if (t >= 1f) {
                        // Shield End of life --> Destroy
                        Destroy(gameObject);
                    }

                    return;
                }
            }

        }

        private void TakeDamage(int damage = 5) {
            _currentHealth = Mathf.Max(_currentHealth - damage, 0);
            Debug.Log($"Shield damaged! Current HP: {_currentHealth}");
        }

        /// <summary>
        /// Initializes the properties that we want to animate based on the curve values for zero damage taken
        /// </summary>
        private void InitializeProperties() {
            var damageTaken = (float)(_maxHealth - _currentHealth) / _maxHealth;
            foreach (var prop in config.shieldProperties) {
                prop.targetValue = prop.curve.Evaluate(damageTaken);
                prop.currentValue = prop.targetValue;
                ApplyProperty(prop);

            }
        }

        /// <summary>
        /// Updates the new property target values based on the total damage taken, so we can smoothly animate them
        /// </summary>
        private void UpdateTargets(float damageTaken01) {
            foreach (var prop in config.shieldProperties) {
                prop.targetValue = prop.curve.Evaluate(damageTaken01);
            }
        }


        /// <summary>
        /// Applies the given property value to the shield material
        /// BreatheStrength is passed to the shieldBreathingCollider to keep the collider radius in sync
        /// </summary>
        private void ApplyProperty(ShieldProperty prop) {
            if (_shieldMaterial == null) {
                return;
            }

            var propertyName = prop.propertyName.ToString();
            if (!_shieldMaterial.HasProperty(propertyName)) {
                return;
            }

            if (propertyName == "_Breathe_Strength") {
                _shieldBreathingCollider.SetNewStrength(prop.currentValue);
            }

            _shieldMaterial.SetFloat(propertyName, prop.currentValue);
        }

        /// <summary>
        /// Defines the current state of the shield animation
        /// </summary>
        private enum AnimationState {
            Spawning,
            Idle,
            FinalHit,
            Explosion
        }
    }
}