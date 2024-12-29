using GamePlay.Common;
using Internal;
using Services;
using UnityEngine;

namespace GamePlay.Levels
{
    public interface IArea : IPaintTarget
    {
        Color Color { get; }
        bool IsAnchor { get; }
        IViewableProperty<bool> IsCompleted { get; }

        void Setup(Color color, RenderMaskData maskData, Transform outlineParent);
    }
}