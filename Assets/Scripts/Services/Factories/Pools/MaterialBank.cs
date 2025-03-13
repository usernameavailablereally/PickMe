using System.Collections.Generic;
using Services.Loaders.Configs;
using UnityEngine;

namespace Services.Factories.Pools
{
    public class MaterialBank 
    {
        private readonly List<MaterialData> _materialAliases;
        private readonly Shader _standardShader;

        public MaterialBank(ColorDefinition[] colorsAliases)
        {
            _materialAliases = new List<MaterialData>(colorsAliases.Length);
            _standardShader = Shader.Find(StringConstants.URP_SHADER_PATH);

            if (_standardShader == null)
            {
                Debug.LogError($"Shader not found: {StringConstants.URP_SHADER_PATH}");
                return;
            }

            CreateMaterials(colorsAliases);
        }

        private void CreateMaterials(ColorDefinition[] colorAlias)
        {
            foreach (ColorDefinition color in colorAlias)
            {
                var material = new Material(_standardShader) { color = color.Color };
                _materialAliases.Add(new MaterialData { Material = material, ColorDefinition = color });
            }
        }

        public MaterialData Get()
        {
            if (_standardShader == null || _materialAliases.Count == 0)
            {
                Debug.LogError("Can't get material. Shader or materials are not initialized.");
                return null;
            }
            int randomIndex = Random.Range(0, _materialAliases.Count);
            MaterialData targetMaterial = _materialAliases[randomIndex];
            _materialAliases.RemoveAt(randomIndex);
            return targetMaterial;
        }
    
        public void Return(MaterialData materialData)
        {
            if (materialData == null) return;
            _materialAliases.Add(materialData);
        }

        public void Clear()
        {
            foreach (MaterialData materialAlias in _materialAliases)
            {
                if (materialAlias != null)
                {
                    Object.Destroy(materialAlias.Material);
                }
            }
        }
    }
}