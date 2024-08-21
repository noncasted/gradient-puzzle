using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintImage
    {
        float Size { get; }
        
        void SetColor(Color color);
        void SetSize(float size);
        void ToCircle();
        void ToRect();
    }
}