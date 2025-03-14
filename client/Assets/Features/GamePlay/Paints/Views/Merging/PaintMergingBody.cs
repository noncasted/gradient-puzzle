using System.Collections.Generic;
using GamePlay.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintMergingBody : MaskableGraphic
    {
        [SerializeField] private AreaRendererData _data;
        [SerializeField] private List<Vector2> _path;

        public void Setup(Color bodyColor)
        {
            color = bodyColor;
        }
        
        public void SetMaterial(Material bodyMaterial)
        {
            material = bodyMaterial;
        }
        
        public void UpdatePath(IReadOnlyList<Vector2> path)
        {
            _path = (List<Vector2>)path;
            _data = path.GetAreaRenderData();
            SetAllDirty();
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