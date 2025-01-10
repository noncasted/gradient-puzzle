using Shapes;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Levels
{
    [DisallowMultipleComponent]
    public class AreaOutlineRenderer : MaskableGraphic
    {
        [SerializeField] private AreaShapeRenderer _shapeRenderer;
        [SerializeField] private AreaRendererOptions _options;
        [SerializeField] private Polyline _line;

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var points = _shapeRenderer.Data.Vertices;

            if (points.Count < 2)
                return;

            for (var i = 0; i < points.Count - 1; i++)
                CreateConnection(points[i], points[i + 1], i);

            CreateConnection(points[^1], points[0], points.Count - 1);

            void CreateConnection(Vector2 from, Vector2 to, int i)
            {
                CreateLineSegment(from, to, vh);

                var index = i * 5;

                vh.AddTriangle(index, index + 1, index + 3);
                vh.AddTriangle(index + 3, index + 2, index);

                if (i != 0)
                {
                    vh.AddTriangle(index, index - 1, index - 3);
                    vh.AddTriangle(index + 1, index - 1, index - 2);
                }
            }
        }

        private void CreateLineSegment(Vector3 point1, Vector3 point2, VertexHelper vh)
        {
            Vector3 offset = rectTransform.sizeDelta / 2;
            var vertex = UIVertex.simpleVert;
            vertex.color = _options.OutlineColor;

            var point1Rotation = Quaternion.Euler(0, 0, RotatePointTowards(point1, point2) + 90);
            vertex.position = point1Rotation * new Vector3(-_options.OutlineWidth / 2, 0);
            vertex.position += point1 - offset;
            vh.AddVert(vertex);
            vertex.position = point1Rotation * new Vector3(_options.OutlineWidth / 2, 0);
            vertex.position += point1 - offset;
            vh.AddVert(vertex);

            var point2Rotation = Quaternion.Euler(0, 0, RotatePointTowards(point2, point1) - 90);
            vertex.position = point2Rotation * new Vector3(-_options.OutlineWidth / 2, 0);
            vertex.position += point2 - offset;
            vh.AddVert(vertex);
            vertex.position = point2Rotation * new Vector3(_options.OutlineWidth / 2, 0);
            vertex.position += point2 - offset;
            vh.AddVert(vertex);

            vertex.position = point2 - offset;
            vh.AddVert(vertex);
        }

        private float RotatePointTowards(Vector2 vertex, Vector2 target)
        {
            return Mathf.Atan2(target.y - vertex.y, target.x - vertex.x) * (180 / Mathf.PI);
        }
    }
}