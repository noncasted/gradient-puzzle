using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class AreaRenderer : MaskableGraphic
    {
        [SerializeField] private AreaRendererData _data;

        public AreaRendererData Data => _data;
        
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