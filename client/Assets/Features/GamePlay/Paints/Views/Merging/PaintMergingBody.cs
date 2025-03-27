using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintMergingBody : MaskableGraphic
    {
        private MergeHandle _handle;

        public void SetColor(Color bodyColor)
        {
            color = bodyColor;
        }

        public void SetMaterial(Material bodyMaterial)
        {
            material = bodyMaterial;
        }

        public void UpdatePath(MergeHandle handle)
        {
            _handle = handle;
            SetAllDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();

            var path = _handle?.GetBodyPath();

            if (path == null || path.Count < 3)
                return;

            path.RenderMergeBody(ref vh, color);
        }
    }
}