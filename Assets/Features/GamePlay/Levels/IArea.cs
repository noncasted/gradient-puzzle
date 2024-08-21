using Features.GamePlay.Common;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IArea : IPaintTarget
    {
        Color Source { get; }
        
        void CheckTouch(Vector2 cursorPosition);
    }
}