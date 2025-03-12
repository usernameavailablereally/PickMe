using System.Collections.Generic;
using UnityEngine;

namespace Services.Factories.Pools
{
    public class MaterialBank
    {
        private readonly List<Material> materials = new List<Material>();
        private readonly Shader standardShader;

        public MaterialBank(Color[] colors)
        {
            standardShader = Shader.Find(StringConstants.URPShaderPath);
            foreach (Color color in colors)
            {
                Material newMaterial = new Material(standardShader) { color = color };
                materials.Add(newMaterial);
            }
        }

        public Material Get()
        {
            if (materials.Count > 0)
            {
                Material material = materials[0];
                materials.RemoveAt(0);
                return material;
            }
            else
            {
                return new Material(standardShader);
            }
        }

        public void ReturnToPool(Material material)
        {
            materials.Add(material);
        }
    }
}