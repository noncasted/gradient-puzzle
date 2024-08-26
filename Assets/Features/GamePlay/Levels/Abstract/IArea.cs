using Features.Services.RenderOptions;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IArea : IPaintTarget
    {
        Color Source { get; }
        bool IsAnchor { get; }

        void Setup(Color color, RenderMaskData maskData);
        void CheckTouch(Vector2 cursorPosition);
    }
}