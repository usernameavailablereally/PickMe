using System.Collections.Generic;
using UnityEngine;

namespace Services.Factories.Pools
{
    public class MaterialBank 
    {
        private readonly List<Material> _materials;
        private readonly Shader _standardShader;

        public MaterialBank(Color[] colors)
        {
            _materials = new List<Material>(colors.Length);
            _standardShader = Shader.Find(StringConstants.URP_SHADER_PATH);

            if (_standardShader == null)
            {
                Debug.LogError($"Shader not found: {StringConstants.URP_SHADER_PATH}");
                return;
            }

            CreateMaterials(colors);
        }

        private void CreateMaterials(Color[] colors)
        {
            foreach (Color color in colors)
            {
                var material = new Material(_standardShader) { color = color };
                _materials.Add(material);
            }
        }

        public Material Get()
        {
            if (_standardShader == null || _materials.Count == 0)
            {
                Debug.LogError("Can't get material. Shader or materials are not initialized.");
                return null;
            }
            int randomIndex = Random.Range(0, _materials.Count);
            Material targetMaterial = _materials[randomIndex];
            _materials.RemoveAt(randomIndex);
            return targetMaterial;
        }
    
        public void Return(Material material)
        {
            if (material == null) return;
            _materials.Add(material);
        }

        public void Clear()
        {
            foreach (Material material in _materials)
            {
                if (material != null)
                {
                    Object.Destroy(material);
                }
            }
        }
    }
}