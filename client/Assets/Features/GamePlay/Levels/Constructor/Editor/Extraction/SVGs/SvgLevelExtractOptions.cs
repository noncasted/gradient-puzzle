using UnityEngine;

namespace GamePlay.Levels.SVGs
{
    public class SvgLevelExtractOptions
    {
        public SvgLevelExtractOptions(
            string svgPath,
            string inkscapePath,
            Vector2 rectSize,
            float pointsDensity)
        {
            SvgPath = svgPath;
            InkscapePath = inkscapePath;
            RectSize = rectSize;
            PointsDensity = pointsDensity;
        }

        public string SvgPath { get; }
        public string InkscapePath { get; }
        public Vector2 RectSize { get; }
        public float PointsDensity { get; }
    }
}