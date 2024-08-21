using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class PaintTransform : MonoBehaviour, IPaintTransform, IEntityComponent
    {
        [SerializeField] private RectTransform _transform;

        public Vector2 Position => _transform.anchoredPosition;

        public void Register(IEntityBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintTransform>();
        }

        public void AttachTo(Transform target)
        {
            _transform.parent = target;
        }

        public void SetRotation(float angle)
        {
            _transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        public void SetPosition(Vector2 position)
        {
            _transform.anchoredPosition = position;
        }
    }
}