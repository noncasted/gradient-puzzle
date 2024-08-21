using UnityEngine;

namespace Features.GamePlay
{
    public interface IArea : IPaintTarget
    {
        Color Source { get; }
        bool IsStartingPoint { get; }

        void Setup(Color color);
        void CheckTouch(Vector2 cursorPosition);
    }
}