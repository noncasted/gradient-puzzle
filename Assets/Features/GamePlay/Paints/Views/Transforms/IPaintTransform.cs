using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintTransform
    {
        Vector2 Position { get; }
        
        void AttachTo(Transform target);
        void SetRotation(float angle);
        void SetPosition(Vector2 position);
    }
}