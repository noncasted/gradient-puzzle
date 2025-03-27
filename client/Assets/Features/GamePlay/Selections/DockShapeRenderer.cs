using GamePlay.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Selections
{
    [DisallowMultipleComponent]
    public class DockShapeRenderer : MaskableGraphic
    {
        [SerializeField] private AreaRendererData _data;

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