using UnityEngine;
using UnityEngine.UI;

namespace Features.GamePlay
{
    public class TestRenderer : MaskableGraphic
    {
        [SerializeField] private AreaRenderer[] _renderers;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();

            foreach (var areaRenderer in _renderers)
                areaRenderer.Data.Render(ref vh, color);
        }
    }
}