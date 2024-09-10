using Features.Services.RenderOptions;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IArea : IPaintTarget
    {
        Color Source { get; }
        bool IsAnchor { get; }
        IViewableProperty<bool> IsCompleted { get; }

        void Setup(Color color, RenderMaskData maskData, Transform outlineParent);
    }
}