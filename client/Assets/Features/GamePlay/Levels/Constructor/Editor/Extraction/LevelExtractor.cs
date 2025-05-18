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

            var previousPoint = Vector2.zero;

            for (var i = 0f; i < length; i += _options.Geometries.PointsDensity)
            {
                var point = properties.GetPointAtLength(i);

                var convertedPoint = new Vector2((float)point.X, (float)point.Y);
                
                var distance = Vector2.Distance(previousPoint, convertedPoint);
                
                if (distance < _options.Geometries.MinDistance)
                    continue;
                
                previousPoint = convertedPoint;
                
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

            var innerCenter = innerPoints.GetCenterOfMass();
            var innerSize = innerPoints.GetSize() / 2f;
            var innerOffset = _options.Geometries.InnerOffset;
            innerOffset *= Mathf.Clamp01(innerSize.magnitude / innerOffset);

            for (var i = 0; i < innerPoints.Count; i++)
            {
                var point = innerPoints[i];

                var offset = point - innerCenter;

                var xDistance = Mathf.Abs(offset.x);
                var yDistance = Mathf.Abs(offset.y);

                var xOffset = innerOffset * Mathf.Clamp01(xDistance / innerSize.x);
                var yOffset = innerOffset * Mathf.Clamp01(yDistance / innerSize.y);

                var xMultiplier = (xDistance - xOffset) / xDistance;
                var yMultiplier = (yDistance - yOffset) / yDistance;

                offset *= new Vector2(xMultiplier, yMultiplier);

                innerPoints[i] = innerCenter + offset;
            }
            
            var contour = new ExtractedArea.Contour(renderPoints, systemPoints, innerPoints);
            return contour;
        }
    }
}