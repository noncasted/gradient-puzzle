using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class AreaRenderer : MaskableGraphic
    {
        [SerializeField] private float _offset;
        [SerializeField] private AreaRendererData _data;
        [SerializeField] private AreaOutlineRenderer _outline;
        
        private IReadOnlyList<Vector2> _points;

        public AreaRendererData Data => _data;
        public AreaOutlineRenderer Outline => _outline;

        public void SetPoints(IReadOnlyList<Vector2> points)
        {
            _data = points.GetAreaRenderData(color);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (_data == null || _data.Vertices.Count < 3)
                return;

            base.OnPopulateMesh(vh);
            vh.Clear();

            _data.Render(ref vh, color);
        }
    }
}