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

        public IReadOnlyList<ExtractedArea> Extract(AreaExtractOptions options)
        {
            var colors = _texture.GetColors(options.ColorEpsilon);
            var areas = new List<ExtractedArea>();

            foreach (var color in colors)
            {
                var area = ExtractColoredArea(options, color);
                areas.Add(area);
            }

            return areas;
        }

        private ExtractedArea ExtractColoredArea(AreaExtractOptions options, AreaExtractedColor sourceColor)
        {
            var pixels = _texture.GetPixels32();
            var textureSize = new Vector2Int(_texture.width, _texture.height);
            var contours = AreaShapeUtils.ExtractContours(pixels, textureSize, options.ErosionPixels, sourceColor);

            foreach (var contour in contours)
            {
                for (var i = 1; i <= options.SimplifyIterations; i++)
                    AreaShapeUtils.SimplifyContour(contour, options.DistanceThreshold * i);

                AreaShapeUtils.NormalizePointsToRect(contour, options.RectSize, textureSize);
            }

            return new ExtractedArea(contours);
        }
    }
}