using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintTarget
    {
        IViewableProperty<bool> IsTouched { get; }
        IPaint Paint { get; }
        Vector2 Position { get; }
        RectTransform Transform { get; }

        void SetPaint(IPaint paint);
        void RemovePaint(IPaint paint);
    }
}