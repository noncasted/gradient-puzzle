using Drawing;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Levels
{
    [InitializeOnLoad]
    public static class AreaGizmosExtensions
    {
        static AreaGizmosExtensions()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
                return;

            foreach (var gameObject in Selection.gameObjects)
            {
                var area = TryGetArea();

                if (area == null)
                    continue;

                area.DrawContour();

                Area TryGetArea()
                {
                    var t = gameObject.GetComponent<Area>();

                    if (t != null)
                        return t;

                    return gameObject.GetComponentInParent<Area>();
                }
            }
        }

        public static void DrawContour(this Area area)
        {
            var offset = new Vector2(540, 960);
            
            using (Draw.WithLineWidth(6))
            {
                foreach (var data in area.Shapes)
                {
                    var points = new NativeArray<float3>(data.RenderPoints.Count, Allocator.Temp);
            
                    for (var i = 0; i < data.RenderPoints.Count; i++)
                    {
                        var point = data.RenderPoints[i] + offset;
                        points[i] = new float3(point.x, point.y, 0f);
                    }
            
                    Draw.Polyline(points, false, Color.red);
                }
            }
            
            using (Draw.WithLineWidth(3))
            {
                foreach (var data in area.Shapes)
                {
                    var points = new NativeArray<float3>(data.SystemPoints.Count, Allocator.Temp);
            
                    for (var i = 0; i < data.SystemPoints.Count; i++)
                    {
                        var point = data.SystemPoints[i] + offset;
                        points[i] = new float3(point.x, point.y, 0f);
                    }
            
                    Draw.Polyline(points, false, Color.green);
                }
            }
        }
    }
}