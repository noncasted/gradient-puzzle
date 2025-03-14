using UnityEngine;

namespace GamePlay.Levels
{
    public class SvgLevelExtractOptions
    {
        public SvgLevelExtractOptions(
            string svgPath,
            string inkscapePath,
            string inkscapeActions,
            Vector2 rectSize,
            Vector2 offset,
            float pointsDensity,
            float simplifyAngle,
            float svgScale,
            float centerCheckDistance)
        {
            SvgPath = svgPath;
            InkscapePath = inkscapePath;
            InkscapeActions = inkscapeActions;
            RectSize = rectSize;
            Offset = offset;
            PointsDensity = pointsDensity;
            SimplifyAngle = simplifyAngle;
            SvgScale = svgScale;
            CenterCheckDistance = centerCheckDistance;
        }

        public string SvgPath { get; }
        public string InkscapePath { get; }
        public string InkscapeActions { get; }
        public Vector2 RectSize { get; }
        public Vector2 Offset { get; }
        public float PointsDensity { get; }
        public float SimplifyAngle { get; }
        public float SvgScale { get; }
        public float CenterCheckDistance { get; }
    }
}