using System.Collections.Generic;
using _Game.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "ColorBlockJam/Color Palette", order = 3)]
    public class ColorPalette : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<ShapeColor, Material> materials = new();

        public Material GetMaterial(ShapeColor color)
        {
            if (materials.TryGetValue(color, out var material))
                return material;

            Debug.LogWarning($"[ColorPalette] No material mapped for {color}.");
            return null;
        }
    }
}
