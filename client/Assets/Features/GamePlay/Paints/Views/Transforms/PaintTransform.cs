using Internal;
using UnityEngine;
using VContainer;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintTransform : MonoBehaviour, IPaintTransform, IEntityComponent
    {
        [SerializeField] private RectTransform _transform;

        private IPaintMoveArea _moveArea;

        public Vector2 RectPosition => _moveArea.Transform.InverseTransformPoint(transform.position);
        public Vector2 WorldPosition => _transform.position;
        public Transform Value => _transform;

        [Inject]
        private void Construct(IPaintMoveArea moveArea)
        {
            _moveArea = moveArea;
        }

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
            var target = _moveArea.Transform.TransformPoint(position);
            target.z = _transform.position.z;
            _transform.position = target;   
        }

        public void SetWorldPosition(Vector2 position)
        {
            _transform.position = new Vector3(position.x, position.y, _transform.position.z);
        }

        public void SetLocalPosition(Vector2 position)
        {
            _transform.localPosition = new Vector3(position.x, position.y);
        }
    }
}