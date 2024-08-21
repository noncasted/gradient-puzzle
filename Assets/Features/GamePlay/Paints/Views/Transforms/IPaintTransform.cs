using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintTransform
    {
        void AttachTo(Transform target);
        void SetRotation(float angle);
        void SetPosition(Vector2 position);
    }
}