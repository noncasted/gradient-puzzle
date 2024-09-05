using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintTransform
    {
        Vector2 RectPosition { get; }
        Vector2 WorldPosition { get; }
        
        void AttachTo(Transform target);
        void SetRotation(float angle);
        void SetRectPosition(Vector2 position);
        void SetWorldPosition(Vector2 position);
    }
}