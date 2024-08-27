using System.Collections.Generic;
using UnityEngine;

namespace Features.GamePlay
{
    public class ExtractedArea
    {
        public ExtractedArea(IReadOnlyList<IReadOnlyList<Vector2>> contours)
        {
            Contours = contours;
        }
        
        public IReadOnlyList<IReadOnlyList<Vector2>> Contours { get; }
    }
}