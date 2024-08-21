using UnityEngine;

namespace Features.GamePlay
{
    public interface IArea : IPaintTarget
    {
        Color Source { get; }
        bool IsStartingPoint { get; }

        void Construct(Color color);
        void CheckTouch(Vector2 cursorPosition);
    }
}