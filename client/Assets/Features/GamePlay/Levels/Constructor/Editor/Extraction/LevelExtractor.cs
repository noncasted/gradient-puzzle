using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePlay.Levels
{
    public class LevelExtractor
    {
        private readonly LevelCreateOptions _options;

        public LevelExtractor(LevelCreateOptions options)
        {
            _options = options;
        }

        public IReadOnlyList<ExtractedArea> Extract()
        {
            var levelDataExtractor = new LevelFileDataExtractor(_options);

            var rawAreas = levelDataExtractor.GetRawAreas();
            var areas = new List<ExtractedArea>();

            Parallel.ForEach(rawAreas, rawArea =>
            {
                var area = Extract(rawArea);
                areas.Add(area);
            });

            areas = areas.OrderBy(t => t.Order).ToList();

            Parallel.ForEach(areas, area =>
            {
                foreach (var contour in area.Contours)
                {
                    var toRemove = new List<Vector2>();

                    foreach (var innerPoint in contour.InnerPoints)
                    {
                        if (contour.SystemPoints.IsInside(innerPoint) == false)
                        {
                            toRemove.Add(innerPoint);
                            continue;
                        }

                        for (var index = areas.Count - 1; index >= 0; index--)
                        {
                            var checkArea = areas[index];

                            if (checkArea == area)
                                break;

                            foreach (var checkContour in checkArea.Contours)
                            {
                                if (checkContour.SystemPoints.IsInside(innerPoint) == false)
                                    continue;

                                toRemove.Add(innerPoint);
                            }
                        }
                    }

                    foreach (var point in toRemove)
                        ((List<Vector2>)contour.InnerPoints).Remove(point);

                    try
                    {
                        ((List<Vector2>)contour.InnerPoints).SimplifyBackwards(_options.Optimizations.System);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error simplifying {area.Name} inner points: {e.Message}");
                    }
                }
            });

            return areas.OrderByDescending(t => t.Order).ToList();
        }

        private ExtractedArea Extract(LevelFileDataExtractor.AreaData areaData)
        {
            var contours = new List<ExtractedArea.Contour>();

            Parallel.ForEach(areaData.Paths, path =>
            {
                var contour = ExtractContour(path);
                contours.Add(contour);
            });

            return new ExtractedArea(contours, areaData.Color, areaData.Order, areaData.Paths[0].Name);
        }

        private ExtractedArea.Contour ExtractContour(LevelFileDataExtractor.PathData pathData)
        {
            var renderPoints = new List<Vector2>();

            var properties = new SvgPathProperties.SvgPath(pathData.D);
            var length = properties.Length;

            for (var i = 0f; i < length; i += _options.Geometries.PointsDensity)
            {
                var point = properties.GetPointAtLength(i);

                var convertedPoint = new Vector2((float)point.X, (float)point.Y);
                convertedPoint.y *= -1f;
                convertedPoint *= _options.Geometries.Scale;

                renderPoints.Add(convertedPoint);
            }

            var centerOffset = new Vector2(-540, 540);

            for (var i = 0; i < renderPoints.Count; i++)
                renderPoints[i] += centerOffset;

            for (var i = 0; i < renderPoints.Count; i++)
                renderPoints[i] += _options.Geometries.Offset;

            if (renderPoints.Count < 4)
                return null;

            var systemPoints = new List<Vector2>();

            systemPoints.AddRange(renderPoints);

            var innerPoints = new List<Vector2>(renderPoints);
            renderPoints.SimplifyForward(_options.Optimizations.Render);
            systemPoints.SimplifyBackwards(_options.Optimizations.System);

            var innerSize = innerPoints.GetSize();
            var halfSize = Mathf.Min(innerSize.x, innerSize.y);
            var offset = Mathf.Min(_options.Geometries.InnerOffset, halfSize);
            var targetSize = innerSize - Vector2.one * offset;
            var scale = targetSize / innerSize;
            var previousInnerCenter = systemPoints.GetCenter();

            for (var i = 0; i < innerPoints.Count; i++)
            {
                var point = innerPoints[i];

                point.x *= scale.x;
                point.y *= scale.y;

                innerPoints[i] = point;
            }

            var currentInnerCenter = innerPoints.GetCenter();
            var innerOffset = (previousInnerCenter - currentInnerCenter);

            for (var i = 0; i < innerPoints.Count; i++)
                innerPoints[i] += innerOffset;

            var contour = new ExtractedArea.Contour(renderPoints, systemPoints, innerPoints);
            return contour;
        }
    }
}