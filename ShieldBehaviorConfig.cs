using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF_wip.SF_Energy_Shield.Scriptables {
    [CreateAssetMenu(menuName = "SF Studio/Shields/ShieldBehaviorConfig")]
    public class ShieldBehaviorConfig : ScriptableObject {
        [field: Header("Shield Settings")]
        [field: Header("Shield Size")]
        [field: SerializeField] 
        public int shieldSize { get; private set; }= 1;

        [field: Header("Hit VFX")]
        [field: SerializeField]
        public GameObject shieldHitPrefab { get; private set; }
        
        [field: Header("Shield Spawn Animation")]
        [field: SerializeField]
        public AnimationCurve initVisibilityCurve { get; private set; }

        [field: SerializeField]
        public AnimationCurve initSizeCurve { get; private set; }

        [field: SerializeField]
        public float initDuration { get; private set; } = 1f;

        [field: Header("Damage State VFX (0 = no damage, 1 = full damage)")]
        [field: SerializeField]
        public List<ShieldProperty> shieldProperties { get; private set; }

        [field: Header("Smoothing")]
        [field: Tooltip("Time in seconds to interpolate between old and new values.")]
        [field: SerializeField]
        public float smoothTime { get; private set; } = 1f;

        [field: Header("Final hit Animation")]
        [field: SerializeField]
        public AnimationCurve positionCurve { get; private set; }

        [field: SerializeField]
        public AnimationCurve strengthCurve { get; private set; }

        [field: SerializeField]
        public float travelDuration { get; private set; } = 1f;

        [field: Header("Explode Animation")]
        [field: SerializeField]
        public AnimationCurve visibilityCurve { get; private set; }

        [field: SerializeField]
        public AnimationCurve hitStrengthCurve { get; private set; }

        [field: SerializeField]
        public float explodeDuration { get; private set; } = 1f;
    }

    [Serializable]
    public class ShieldProperty {
        public ShieldPropertyName propertyName;
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        [HideInInspector]
        // Smoothed value
        public float currentValue;
        [HideInInspector]
        public float targetValue;
    }

    /// <summary>
    /// Note: currently only float values supported!
    /// </summary>
    [Serializable]
    public enum ShieldPropertyName
    {
        _Cloud_Brightness,
        _Cloud_Speed,
        _Breathe_Frequency, //1
        _Instability_Strength, //2
        _Instability_Speed,
        _Default_Face_Size,
        _Shrink_Size,
        _Rotation_Speed,//4
        _Crit_Distortion_Strength,//3
        _Visibility
        // Add more if you need, but add it at the bottom of the enum to not mess up existing configurations
    }
}