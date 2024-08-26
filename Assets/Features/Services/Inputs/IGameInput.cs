using Internal;
using UnityEngine;

namespace Features.Services.Inputs
{
    public interface IGameInput
    {
        IViewableProperty<bool> Action { get;  }
        Vector2 CursorPosition { get; }
        
        Vector2 GetInputInRect(RectTransform rect);
    }
}