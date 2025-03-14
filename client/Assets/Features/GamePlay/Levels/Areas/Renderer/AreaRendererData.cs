﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Levels
{
    [Serializable]
    public class AreaRendererData
    {
        [SerializeField] private List<Vector2> _vertices;
        [SerializeField] private List<int> _triangles;

        public List<Vector2> Vertices => _vertices;
        public List<int> Triangles => _triangles;

        public AreaRendererData(List<Vector2> vertices, List<int> triangles)
        {
            _vertices = vertices;
            _triangles = triangles;
        }

        public AreaRendererData()
        {
            _vertices = new List<Vector2>();
            _triangles = new List<int>();
        }
    }

    public static class AreaRendererExtensions
    {
        public static AreaRendererData GetAreaRenderData(this IReadOnlyList<Vector2> path)
        {
            var isClockwise = PolygonSignedArea(path) > 0;

            var clockwiseSign = isClockwise ? 1f : -1f;
            var pointCount = path.Count;

            if (pointCount < 2)
                return new AreaRendererData();

            var triangleCount = pointCount - 2;
            var triangleIndexCount = triangleCount * 3;
            var meshTriangles = new int[triangleIndexCount];

            var pointsLeft = new List<EarClipPoint>(pointCount);

            for (var i = 0; i < pointCount; i++)
                pointsLeft.Add(new EarClipPoint(i, new Vector2(path[i].x, path[i].y), isClockwise));

            for (var i = 0; i < pointCount; i++)
            {
                var p = pointsLeft[i];
                p.Previous = pointsLeft[(i + pointCount - 1) % pointCount];
                p.Next = pointsLeft[(i + 1) % pointCount];
            }

            var triangleIndex = 0;
            int countLeft;
            var safeguard = 1000000;

            while ((countLeft = pointsLeft.Count) >= 3 && (safeguard-- > 0))
            {
                if (countLeft == 3)
                {
                    meshTriangles[triangleIndex++] = pointsLeft[2].VertexIndex;
                    meshTriangles[triangleIndex++] = pointsLeft[1].VertexIndex;
                    meshTriangles[triangleIndex] = pointsLeft[0].VertexIndex;
                    break;
                }

                var foundConvex = false;

                for (var i = 0; i < countLeft; i++)
                {
                    var point = pointsLeft[i];

                    if (point.ReflexState != ReflexState.Convex)
                        continue;

                    var canClipEar = true;
                    var idPrev = (i + countLeft - 1) % countLeft;
                    var idNext = (i + 1) % countLeft;
                    for (var j = 0; j < countLeft; j++)
                    {
                        if (j == i || j == idPrev || j == idNext)
                            continue;

                        if (pointsLeft[j].ReflexState != ReflexState.Reflex)
                            continue;

                        if (point.IsPointInside(pointsLeft[j], 0f, clockwiseSign * -0.0001f) == false)
                            continue;

                        canClipEar = false;
                        break;
                    }

                    if (canClipEar == false)
                        continue;

                    meshTriangles[triangleIndex++] = point.Next.VertexIndex;
                    meshTriangles[triangleIndex++] = point.VertexIndex;
                    meshTriangles[triangleIndex++] = point.Previous.VertexIndex;
                    point.Next.MarkReflexUnknown();
                    point.Previous.MarkReflexUnknown();
                    (point.Next.Previous, point.Previous.Next) = (point.Previous, point.Next);
                    pointsLeft.RemoveAt(i);
                    foundConvex = true;
                    break;
                }

                if (foundConvex == false)
                {
                    var s =
                        "Invalid polygon triangulation - no convex edges found. Your polygon is likely self-intersecting.\n";
                    s += "Failed point set:\n";
                    s += string.Join("\n", pointsLeft.Select(p => $"[{p.VertexIndex}]: {p.ReflexState}"));
                    Debug.LogError(s);

                    var newPath = new List<Vector2>(path);
                    var offset = 0;

                    foreach (var left in pointsLeft)
                    {
                        newPath.RemoveAt(left.VertexIndex - offset);
                        offset++;
                    }

                    return newPath.GetAreaRenderData();
                }
            }

            return new AreaRendererData(path.ToList(), meshTriangles.ToList());
        }

        public static void Render(this AreaRendererData data, ref VertexHelper vertexHelper, Color color)
        {
            var vertices = new List<UIVertex>(data.Vertices.Count);

            for (var i = 0; i < data.Vertices.Count; i++)
            {
                var v = UIVertex.simpleVert;
                v.color = color;
                v.position = data.Vertices[i];
                vertices.Add(v);
            }

            vertexHelper.AddUIVertexStream(vertices, data.Triangles);
        }

        private static float PolygonSignedArea(IReadOnlyList<Vector2> pts)
        {
            var count = pts.Count;
            var sum = 0f;
            for (var i = 0; i < count; i++)
            {
                var a = pts[i];
                var b = pts[(i + 1) % count];
                sum += (b.x - a.x) * (b.y + a.y);
            }

            return sum;
        }
    }
}