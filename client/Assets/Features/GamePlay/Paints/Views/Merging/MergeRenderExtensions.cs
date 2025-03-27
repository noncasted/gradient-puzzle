using System.Collections.Generic;
using GamePlay.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Paints
{
    public static class MergeRenderExtensions
    {
        public static void RenderMergeBody(this List<UIVertex> path, ref VertexHelper vh, Color color)
        {
            var isClockwise = PolygonSignedArea(path) > 0;

            var clockwiseSign = isClockwise ? 1f : -1f;
            var pointCount = path.Count;

            if (pointCount < 2)
                return;

            var triangleCount = pointCount - 2;
            var triangleIndexCount = triangleCount * 3;
            var meshTriangles = new List<int>(triangleIndexCount);

            for (var i = 0; i < triangleIndexCount; i++)
                meshTriangles.Add(0);

            var pointsLeft = new List<EarClipPoint>(pointCount);

            for (var i = 0; i < pointCount; i++)
                pointsLeft.Add(new EarClipPoint(i, new Vector2(path[i].position.x, path[i].position.y), isClockwise));

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
                    Debug.LogWarning(
                        "Invalid polygon triangulation - no convex edges found. Your polygon is likely self-intersecting.");

                    return;
                }
            }

            for (var index = 0; index < path.Count; index++)
            {
                var point = path[index];
                point.color = color;
                path[index] = point;
            }

            vh.AddUIVertexStream(path, meshTriangles);
        }

        private static float PolygonSignedArea(IReadOnlyList<UIVertex> pts)
        {
            var count = pts.Count;
            var sum = 0f;
            for (var i = 0; i < count; i++)
            {
                var a = pts[i];
                var b = pts[(i + 1) % count];
                sum += (b.position.x - a.position.x) * (b.position.y + a.position.y);
            }

            return sum;
        }
    }
}