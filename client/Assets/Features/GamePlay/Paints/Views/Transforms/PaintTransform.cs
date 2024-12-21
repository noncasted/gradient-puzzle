using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintTransform : MonoBehaviour, IPaintTransform, IEntityComponent
    {
        [SerializeField] private RectTransform _transform;

        public Vector2 RectPosition => _transform.anchoredPosition;
        public Vector2 WorldPosition => _transform.position;

        public void Register(IEntityBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintTransform>();
        }

        public void AttachTo(Transform target)
        {
            _transform.SetParent(target);
        }

        public void SetRotation(float angle)
        {
            _transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        public void SetRectPosition(Vector2 position)
        {
            _transform.anchoredPosition = position;
        }

        public void SetWorldPosition(Vector2 position)
        {
            _transform.position = new Vector3(position.x, position.y, _transform.position.z);
        }
    }
}