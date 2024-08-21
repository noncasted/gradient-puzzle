using Global.UI;
using Internal;
using MPUIKIT;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class PaintImage : MonoBehaviour, IPaintImage, IEntityComponent
    {
        [SerializeField] private MPImage _image;
        [SerializeField] private RectTransform _transform;

        public float Size => _transform.rect.size.x;

        public void Register(IEntityBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintImage>();
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public void SetRadius(float radius)
        {
            var circle = _image.Circle;
            circle.Radius = radius;
            _image.Circle = circle;
        }

        public void SetSize(float size)
        {
            _transform.sizeDelta = new Vector2(size, size);
        }

        public void ToCircle()
        {
            _image.DrawShape = DrawShape.Circle;
            _transform.SetAnchor(AnchorPresets.StretchAll);
        }

        public void ToRect()
        {
            _image.DrawShape = DrawShape.Rectangle;
            _transform.SetAnchor(AnchorPresets.MiddleCenter);
        }
    }
}