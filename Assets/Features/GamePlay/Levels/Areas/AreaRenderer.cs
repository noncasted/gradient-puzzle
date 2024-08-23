using System;
using System.Collections.Generic;
using System.Linq;
using Shapes;
using UnityEngine;
using UnityEngine.UI;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class AreaRenderer : MaskableGraphic
    {
        [SerializeField] private Vector2[] _points;

        public void SetPoints(IReadOnlyList<Vector2> points)
        {
            _points = points.ToArray();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (_points == null || _points.Length < 3)
                return;

            base.OnPopulateMesh(vh);
            vh.Clear();

            GenPolygonMesh(ref vh, _points.ToList(), PolygonTriangulation.EarClipping);
        }

        public class EarClipPoint
        {
            public int vertIndex;
            public Vector2 pt;
            ReflexState reflex = ReflexState.Unknown;

            public EarClipPoint prev;
            public EarClipPoint next;

            public EarClipPoint(int vertIndex, Vector2 pt)
            {
                this.vertIndex = vertIndex;
                this.pt = pt;
            }

            public void MarkReflexUnknown() => reflex = ReflexState.Unknown;

            public ReflexState ReflexState
            {
                get
                {
                    if (reflex == ReflexState.Unknown)
                    {
                        var dirNext = GetDirection(pt, next.pt);
                        var dirPrev = GetDirection(prev.pt, pt);
                        var cwSign = _isClockwise ? 1 : -1;
                        reflex = cwSign * ShapesMath.Determinant(dirPrev, dirNext) >= -0.001f
                            ? ReflexState.Reflex
                            : ReflexState.Convex;
                    }

                    return reflex;
                }
            }
        }

        private static bool _isClockwise;

        public static Vector2 GetDirection(Vector2 a, Vector2 b)
        {
            var dx = b.x - a.x;
            var dy = b.y - a.y;
            var mag = Mathf.Sqrt(dx * dx + dy * dy);
            return new Vector2(dx / mag, dy / mag);
        }

        public enum ReflexState
        {
            Unknown,
            Reflex,
            Convex
        }

        public void GenPolygonMesh(ref VertexHelper vh, List<Vector2> path, PolygonTriangulation triangulation)
        {
            _isClockwise = ShapesMath.PolygonSignedArea(path) > 0;
            var clockwiseSign = _isClockwise ? 1f : -1f;
            var pointCount = path.Count;
            if (pointCount < 2)
                return;

            var triangleCount = pointCount - 2;
            var triangleIndexCount = triangleCount * 3;
            var meshTriangles = new int[triangleIndexCount];

            if (triangulation == PolygonTriangulation.FastConvexOnly)
            {
                var tri = 0;
                for (var i = 0; i < triangleCount; i++)
                {
                    meshTriangles[tri++] = i + 2;
                    meshTriangles[tri++] = i + 1;
                    meshTriangles[tri++] = 0;
                }
            }
            else
            {
                var pointsLeft = new List<EarClipPoint>(pointCount);

                for (var i = 0; i < pointCount; i++)
                    pointsLeft.Add(new EarClipPoint(i, new Vector2(path[i].x, path[i].y)));

                for (var i = 0; i < pointCount; i++)
                {
                    var p = pointsLeft[i];
                    p.prev = pointsLeft[(i + pointCount - 1) % pointCount];
                    p.next = pointsLeft[(i + 1) % pointCount];
                }

                var triangleIndex = 0;
                int countLeft;
                var safeguard = 1000000;

                while ((countLeft = pointsLeft.Count) >= 3 && (safeguard-- > 0))
                {
                    if (countLeft == 3)
                    {
                        meshTriangles[triangleIndex++] = pointsLeft[2].vertIndex;
                        meshTriangles[triangleIndex++] = pointsLeft[1].vertIndex;
                        meshTriangles[triangleIndex] = pointsLeft[0].vertIndex;
                        break;
                    }

                    var foundConvex = false;
                    for (var i = 0; i < countLeft; i++)
                    {
                        var p = pointsLeft[i];
                        if (p.ReflexState == ReflexState.Convex)
                        {
                            var canClipEar = true;
                            var idPrev = (i + countLeft - 1) % countLeft;
                            var idNext = (i + 1) % countLeft;
                            for (var j = 0; j < countLeft; j++)
                            {
                                if (j == i) continue;
                                if (j == idPrev) continue;
                                if (j == idNext) continue;
                                if (pointsLeft[j].ReflexState == ReflexState.Reflex)
                                {
                                    if (ShapesMath.PointInsideTriangle(p.next.pt, p.pt, p.prev.pt, pointsLeft[j].pt, 0f,
                                            clockwiseSign * -0.0001f, 0f))
                                    {
                                        canClipEar = false;
                                        break;
                                    }
                                }
                            }

                            if (canClipEar)
                            {
                                meshTriangles[triangleIndex++] = p.next.vertIndex;
                                meshTriangles[triangleIndex++] = p.vertIndex;
                                meshTriangles[triangleIndex++] = p.prev.vertIndex;
                                p.next.MarkReflexUnknown();
                                p.prev.MarkReflexUnknown();
                                (p.next.prev, p.prev.next) = (p.prev, p.next);
                                pointsLeft.RemoveAt(i);
                                foundConvex = true;
                                break;
                            }
                        }
                    }

                    if (foundConvex == false)
                    {
                        var s =
                            "Invalid polygon triangulation - no convex edges found. Your polygon is likely self-intersecting.\n";
                        s += "Failed point set:\n";
                        s += string.Join("\n", pointsLeft.Select(p => $"[{p.vertIndex}]: {p.ReflexState}"));
                        throw new Exception(s);
                    }
                }
            }

            var verts3D = new List<UIVertex>(pointCount);
            for (var i = 0; i < pointCount; i++)
            {
                var v = UIVertex.simpleVert;
                v.color = color;
                v.position = path[i];
                verts3D.Add(v);
            }

            vh.AddUIVertexStream(verts3D, meshTriangles.ToList());
        }
    }
}