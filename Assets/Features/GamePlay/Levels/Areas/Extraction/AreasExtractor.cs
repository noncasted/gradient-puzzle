using System.Collections.Generic;
using UnityEngine;

namespace Features.GamePlay
{
    public class AreasExtractor
    {
        public AreasExtractor(Texture2D texture)
        {
            _texture = texture;
        }

        private readonly Texture2D _texture;

        public IReadOnlyList<AreaData> Extract(AreaExtractOptions options)
        {
            var colors = _texture.GetColors(options.ColorEpsilon);
            var areas = new List<AreaData>();

            foreach (var color in colors)
            {
                var area = ExtractColoredArea(options, color);
                areas.Add(area);
            }

            return areas;
        }

        private AreaData ExtractColoredArea(AreaExtractOptions options, AreaExtractedColor sourceColor)
        {
            var pixels = _texture.GetPixels32();
            var textureSize = new Vector2Int(_texture.width, _texture.height);
            var contour = AreaShapeUtils.ExtractContour(pixels, textureSize, options.ErosionPixels, sourceColor);

            for (var i = 1; i <= options.SimplifyIterations; i++)
                contour = AreaShapeUtils.SimplifyContour(contour, options.DistanceThreshold * i);

            var normalizedPoints = AreaShapeUtils.NormalizePointsToRect(contour, options.RectSize, textureSize);
            var center = AreaShapeUtils.GetCenter(normalizedPoints);
            var color = AreaDataExtensions.GetInterpolatedColor(center, options.RectSize.x, options.LevelColors);

            return new AreaData(normalizedPoints.ToArray(), center, color);
        }
    }
}