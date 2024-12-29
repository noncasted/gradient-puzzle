using UnityEngine;

namespace GamePlay.Levels.SVGs
{
    public class SvgLevelExtractOptions
    {
        public SvgLevelExtractOptions(
            string svgPath,
            string inkscapePath,
            string inkscapeActions,
            Vector2 rectSize,
            float pointsDensity,
            float simplifyAngle, 
            float svgScale)
        {
            SvgPath = svgPath;
            InkscapePath = inkscapePath;
            InkscapeActions = inkscapeActions;
            RectSize = rectSize;
            PointsDensity = pointsDensity;
            SimplifyAngle = simplifyAngle;
            SvgScale = svgScale;
        }

        public string SvgPath { get; }
        public string InkscapePath { get; }
        public string InkscapeActions { get; }
        public Vector2 RectSize { get; }
        public float PointsDensity { get; }
        public float SimplifyAngle { get; }
        public float SvgScale { get; }
    }
}