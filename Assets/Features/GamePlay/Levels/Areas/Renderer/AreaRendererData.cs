using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Features.GamePlay
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
        public static float PolygonSignedArea(IReadOnlyList<Vector2> pts)
        {
            int count = pts.Count;
            float sum = 0f;
            for (int i = 0; i < count; i++)
            {
                Vector2 a = pts[i];
                Vector2 b = pts[(i + 1) % count];
                sum += (b.x - a.x) * (b.y + a.y);
            }

            return sum;
        }

        public static AreaRendererData GetAreaRenderData(this IReadOnlyList<Vector2> path, Color color)
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
                    return new AreaRendererData();
                }
            }

            return new AreaRendererData(path.ToList(), meshTriangles.ToList());
        }

        public static AreaOutlineData GetOutlineData(this IReadOnlyList<Vector2> path, bool flattenZ, float thickness)
        {
            int pointCount = path.Count;

            if (pointCount < 2)
                return new AreaOutlineData();
            
            var firstPoint = path[0];
            var lastPoint = path[^2];

            // only mitered joints can be in the same submesh at the moment
            bool separateJoinMesh = true;
            bool isSimpleJoin = false; // only used when join meshes exist
            int vertsPerPathPoint = separateJoinMesh ? 5 : 2;
            int vertexCount = pointCount * vertsPerPathPoint;

            // Joins mesh data
            int joinVertsPerJoin = isSimpleJoin ? 3 : 5;

            // indices used per triangle
            int iv0, iv1, iv2 = 0, iv3 = 0, iv4 = 0;
            int ivj0 = 0, ivj1 = 0, ivj2 = 0, ivj3 = 0, ivj4 = 0;
            int triId = 0;
            int triIdJoin = 0;
            var meshVertices = new ExpandableList<Vector2>();

            var meshUv0 = new ExpandableList<Vector4>();
            var meshUv1Prevs = new ExpandableList<Vector2>();
            var meshUv2Nexts = new ExpandableList<Vector2>();
            var meshTriangles = new ExpandableList<int>();
            var meshJoinsTriangles = new ExpandableList<int>();
            
            for (int i = 0; i < pointCount; i++)
            {
                bool isLast = i == pointCount - 1;
                bool isFirst = i == 0;
                bool makeJoin =   (!isLast && !isFirst);
                bool isEndpoint = (isFirst || isLast);
                float uvEndpointValue = isEndpoint ? (isFirst ? -1 : 1) : 0;

                // Indices & verts
                Vector3 vert = flattenZ ? new Vector3(path[i].x, path[i].y, 0f) : path[i];

                iv0 = i * vertsPerPathPoint;

                iv1 = iv0 + 1; // "prev" outer
                iv2 = iv0 + 2; // "next" outer
                iv3 = iv0 + 3; // "prev" inner
                iv4 = iv0 + 4; // "next" inner
                meshVertices[iv0] = vert;
                meshVertices[iv1] = vert;
                meshVertices[iv2] = vert;
                meshVertices[iv3] = vert;
                meshVertices[iv4] = vert;

                // joins mesh
                if (makeJoin)
                {
                    int joinIndex = (i - 1); // Skip first if open
                    ivj0 = joinIndex * joinVertsPerJoin + vertexCount;
                    ivj1 = ivj0 + 1;
                    ivj2 = ivj0 + 2;
                    ivj3 = ivj0 + 3;
                    ivj4 = ivj0 + 4;
                    meshVertices[ivj0] = vert;
                    meshVertices[ivj1] = vert;
                    meshVertices[ivj2] = vert;

                    if (isSimpleJoin == false)
                    {
                        meshVertices[ivj3] = vert;
                        meshVertices[ivj4] = vert;
                    }
                }


                // Setting up next/previous positions
                Vector3 prevPos;
                Vector3 nextPos;
                
                if (i == 0)
                {
                    prevPos = lastPoint; // Mirror second point
                    nextPos = path[i + 1];
                }
                else if (i == pointCount - 1)
                {
                    prevPos = path[i - 1];
                    nextPos = firstPoint; // Mirror second last point
                }
                else
                {
                    prevPos = path[i - 1];
                    nextPos = path[i + 1];
                }

                void SetPrevNext(int atIndex)
                {
                    meshUv1Prevs[atIndex] = prevPos;
                    meshUv2Nexts[atIndex] = nextPos;
                }

                SetPrevNext(iv0);
                SetPrevNext(iv1);
                
                if (separateJoinMesh)
                {
                    SetPrevNext(iv2);
                    SetPrevNext(iv3);
                    SetPrevNext(iv4);
                    if (makeJoin)
                    {
                        SetPrevNext(ivj0);
                        SetPrevNext(ivj1);
                        SetPrevNext(ivj2);
                        if (isSimpleJoin == false)
                        {
                            SetPrevNext(ivj3);
                            SetPrevNext(ivj4);
                        }
                    }
                }

                void SetUv0(ExpandableList<Vector4> uvArr, float uvEndpointVal, float pathThicc, int id, float x,
                    float y) => uvArr[id] = new Vector4(x, y, uvEndpointVal, pathThicc);

                if (separateJoinMesh)
                {
                    SetUv0(meshUv0, uvEndpointValue, thickness, iv0, 0, 0);
                    SetUv0(meshUv0, uvEndpointValue, thickness, iv1, -1, -1);
                    SetUv0(meshUv0, uvEndpointValue, thickness, iv2, -1, 1);
                    SetUv0(meshUv0, uvEndpointValue, thickness, iv3, 1, -1);
                    SetUv0(meshUv0, uvEndpointValue, thickness, iv4, 1, 1);
                    if (makeJoin)
                    {
                        SetUv0(meshUv0, uvEndpointValue, thickness, ivj0, 0, 0);
                        if (isSimpleJoin)
                        {
                            SetUv0(meshUv0, uvEndpointValue, thickness, ivj1, 1, -1);
                            SetUv0(meshUv0, uvEndpointValue, thickness, ivj2, 1, 1);
                        }
                        else
                        {
                            SetUv0(meshUv0, uvEndpointValue, thickness, ivj1, 1, -1);
                            SetUv0(meshUv0, uvEndpointValue, thickness, ivj2, -1, -1);
                            SetUv0(meshUv0, uvEndpointValue, thickness, ivj3, -1, 1);
                            SetUv0(meshUv0, uvEndpointValue, thickness, ivj4, 1, 1);
                        }
                    }
                }
                else
                {
                    SetUv0(meshUv0, uvEndpointValue, thickness, iv0, -1, i);
                    SetUv0(meshUv0, uvEndpointValue, thickness, iv1, 1, i);
                }


                if (isLast == false)
                {
                    // clockwise order
                    void AddQuad(int a, int b, int c, int d)
                    {
                        meshTriangles[triId++] = a;
                        meshTriangles[triId++] = b;
                        meshTriangles[triId++] = c;
                        meshTriangles[triId++] = c;
                        meshTriangles[triId++] = d;
                        meshTriangles[triId++] = a;
                    }

                    if (separateJoinMesh)
                    {
                        int rootCenter = iv0;
                        int rootOuter = iv2;
                        int rootInner = iv4;
                        int nextCenter = isLast ? 0 : rootCenter + vertsPerPathPoint;
                        int nextOuter = nextCenter + 1;
                        int nextInner = nextCenter + 3;
                        AddQuad(rootCenter, rootOuter, nextOuter, nextCenter);
                        AddQuad(nextCenter, nextInner, rootInner, rootCenter);

                        if (makeJoin)
                        {
                            meshJoinsTriangles[triIdJoin++] = ivj0;
                            meshJoinsTriangles[triIdJoin++] = ivj1;
                            meshJoinsTriangles[triIdJoin++] = ivj2;

                            if (isSimpleJoin == false)
                            {
                                meshJoinsTriangles[triIdJoin++] = ivj2;
                                meshJoinsTriangles[triIdJoin++] = ivj3;
                                meshJoinsTriangles[triIdJoin++] = ivj0;

                                meshJoinsTriangles[triIdJoin++] = ivj0;
                                meshJoinsTriangles[triIdJoin++] = ivj3;
                                meshJoinsTriangles[triIdJoin++] = ivj4;
                            }
                        }
                    }
                }
            }

            
            return new AreaOutlineData(meshVertices.list, meshTriangles.list);
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
        
        public static void RenderOutline(this AreaOutlineData data, ref VertexHelper vertexHelper, Color color)
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
    }
}