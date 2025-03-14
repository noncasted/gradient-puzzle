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
            float renderSimplifyAngle,
            float svgScale,
            float centerCheckDistance,
            float systemSimplifyAngle)
        {
            SvgPath = svgPath;
            InkscapePath = inkscapePath;
            InkscapeActions = inkscapeActions;
            RectSize = rectSize;
            Offset = offset;
            PointsDensity = pointsDensity;
            RenderSimplifyAngle = renderSimplifyAngle;
            SvgScale = svgScale;
            CenterCheckDistance = centerCheckDistance;
            SystemSimplifyAngle = systemSimplifyAngle;
        }

        public string SvgPath { get; }
        public string InkscapePath { get; }
        public string InkscapeActions { get; }
        public Vector2 RectSize { get; }
        public Vector2 Offset { get; }
        public float PointsDensity { get; }
        public float RenderSimplifyAngle { get; }
        public float SystemSimplifyAngle { get; }
        public float SvgScale { get; }
        public float CenterCheckDistance { get; }
    }
}