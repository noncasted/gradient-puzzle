using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public class ExtractedArea
    {
        public ExtractedArea(
            IReadOnlyList<IReadOnlyList<Vector2>> contours,
            Color color,
            int order,
            string name)
        {
            Contours = contours;
            Color = color;
            Order = order;
            Name = name;
        }
        
        public IReadOnlyList<IReadOnlyList<Vector2>> Contours { get; }
        public Color Color { get; }
        public int Order { get; }
        public string Name { get; }
    }
}