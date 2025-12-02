using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpaceFusion.SF_Energy_Shield.Scripts {
    /// <summary>
    /// Copies the defined source material properties to the target material
    /// This is useful when the shield has some breathe and instability effects enabled to adapt the hit VFX
    /// Otherwise it can appear that the hit vfx is shown too much inside (or outside) the shield bubble
    /// </summary>
    public class CopyMaterialProperties : MonoBehaviour {
        [Header("Defines properties to be copied")]
        private readonly string[] propertiesToCopy = {
            "_Breathe_Frequency",
            "_Breathe_Strength",
            "_Phase",
            "_Instability_Strength",
            "_Instability_Speed",
        };

        [ContextMenu("Copy Properties Now")]
        public void CopyProperties(Material sourceMaterial, Material targetMaterial) {
            if (sourceMaterial == null || targetMaterial == null || propertiesToCopy.Length == 0) {
                Debug.LogWarning("Source or Target material or list of props to copy is missing!");
                return;
            }

            var shader = sourceMaterial.shader;
            var propertyCount = shader.GetPropertyCount();

            for (var i = 0; i < propertyCount; i++) {
                var propertyName = shader.GetPropertyName(i);

                // Skip excluded properties
                if (!Array.Exists(propertiesToCopy, p => p == propertyName)) {
                    continue;
                }

                var propertyType = shader.GetPropertyType(i);

                switch (propertyType) {
                    case ShaderPropertyType.Color:
                        targetMaterial.SetColor(propertyName, sourceMaterial.GetColor(propertyName));
                        break;

                    case ShaderPropertyType.Vector:
                        targetMaterial.SetVector(propertyName, sourceMaterial.GetVector(propertyName));
                        break;

                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        targetMaterial.SetFloat(propertyName, sourceMaterial.GetFloat(propertyName));
                        break;

                    case ShaderPropertyType.Texture:
                        targetMaterial.SetTexture(propertyName, sourceMaterial.GetTexture(propertyName));
                        targetMaterial.SetTextureOffset(propertyName, sourceMaterial.GetTextureOffset(propertyName));
                        targetMaterial.SetTextureScale(propertyName, sourceMaterial.GetTextureScale(propertyName));
                        break;
                    case ShaderPropertyType.Int:
                        targetMaterial.SetInt(propertyName, sourceMaterial.GetInt(propertyName));
                        break;
                }
            }
        }
    }
}