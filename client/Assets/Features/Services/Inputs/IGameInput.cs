using Internal;
using UnityEngine;

namespace Services
{
    public interface IGameInput
    {
        IViewableProperty<bool> Action { get;  }
        Vector2 CursorPosition { get; }
        Vector2 WorldPosition { get; }
        
        Vector2 GetInputInRect(RectTransform rect);
    }
}