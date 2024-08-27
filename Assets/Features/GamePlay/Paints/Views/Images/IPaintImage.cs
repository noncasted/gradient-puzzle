using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintImage
    {
        float Size { get; }
        Color Color { get; }

        void SetColor(Color color);
        void SetSize(float size);
        void SetMaterial(Material material);
        void ResetMaterial();
    }
}