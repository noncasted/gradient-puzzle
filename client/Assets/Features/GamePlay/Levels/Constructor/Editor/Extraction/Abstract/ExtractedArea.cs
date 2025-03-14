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
            public Contour(IReadOnlyList<Vector2> points, IReadOnlyList<Vector2> centers)
            {
                Points = points;
                Centers = centers;
            }

            public IReadOnlyList<Vector2> Points { get; }
            public IReadOnlyList<Vector2> Centers { get; }
        }
    }
}