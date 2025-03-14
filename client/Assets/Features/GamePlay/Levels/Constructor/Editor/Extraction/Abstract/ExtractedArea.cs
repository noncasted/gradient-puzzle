using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public class ExtractedArea
    {
        public ExtractedArea(
            IReadOnlyList<Contour> contours,
            Color color,
            int order,
            string name)
        {
            Contours = contours;
            Color = color;
            Order = order;
            Name = name;
        }

        public IReadOnlyList<Contour> Contours { get; }
        public Color Color { get; }
        public int Order { get; }
        public string Name { get; }

        public class Contour
        {
            public Contour(
                IReadOnlyList<Vector2> renderPoints,
                IReadOnlyList<Vector2> systemPoints,
                IReadOnlyList<Vector2> centers)
            {
                RenderPoints = renderPoints;
                SystemPoints = systemPoints;
                Centers = centers;
            }

            public IReadOnlyList<Vector2> RenderPoints { get; }
            public IReadOnlyList<Vector2> SystemPoints { get; }
            public IReadOnlyList<Vector2> Centers { get; }
        }
    }
}