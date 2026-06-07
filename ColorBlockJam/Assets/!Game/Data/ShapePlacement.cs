using System;
using _Game.Enums;
using UnityEngine;

namespace _Game.Data
{
    [Serializable]
    public class ShapePlacement
    {
        public ShapeDefinition definition;
        public ProductionColor color = ProductionColor.None;
        public Vector2Int anchor;
        public ShapeRotation rotation;
    }
}
