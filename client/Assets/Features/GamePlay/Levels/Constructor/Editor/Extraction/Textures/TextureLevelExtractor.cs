using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public class TextureLevelExtractor
    {
        public TextureLevelExtractor(Texture2D texture)
        {
            _texture = texture;
        }

        private readonly Texture2D _texture;

        public IReadOnlyList<ExtractedArea> Extract(TextureLevelExtractOptions options)
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

        private ExtractedArea ExtractColoredArea(TextureLevelExtractOptions options, AreaExtractedColor sourceColor)
        {
            var pixels = _texture.GetPixels32();
            var textureSize = new Vector2Int(_texture.width, _texture.height);
            var contours =
                TextureLevelExtractorExtensions.ExtractContours(pixels, textureSize, options.ErosionPixels,
                    sourceColor);

            foreach (var contour in contours)
            {
                for (var i = 1; i <= options.SimplifyIterations; i++)
                    AreaShapeUtils.SimplifyContour(contour, options.DistanceThreshold * i);

                AreaShapeUtils.NormalizePointsToRect(contour, options.RectSize, textureSize);
            }

            return new ExtractedArea(contours, Color.white, 0, "Area");
        }
    }
}