using UnityEngine;

namespace GamePlay.Paints
{
    public interface IPaintFill
    {
        Vector2 RectPosition { get; }
        
        void SetColor(Color color);
        void SetMaterial(Material material);
        void ResetMaterial();
        void SetWorldPosition(Vector2 position);
        void SetRectPosition(Vector2 position);
        void SetSize(float size);
    }
}