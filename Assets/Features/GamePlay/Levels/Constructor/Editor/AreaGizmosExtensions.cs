using Drawing;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Features.GamePlay
{
    public static class AreaGizmosExtensions
    {
        public static void DrawContour(this Area area)
        {
            var offset = new Vector2(540, 960);

            using (Draw.WithLineWidth(1.5f))
            {
                foreach (var data in area.Datas)
                {
                    var points = new NativeArray<float3>(data.Points.Count, Allocator.Temp);

                    for (var i = 0; i < data.Points.Count; i++)
                    {
                        var point = data.Points[i] + offset;
                        points[i] = new float3(point.x, point.y, 0f);
                    }

                    Draw.Polyline(points, false, Color.red);
                }
            }
        }
    }
}