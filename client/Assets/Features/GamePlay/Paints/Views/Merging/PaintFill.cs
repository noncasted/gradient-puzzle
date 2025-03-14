using Internal;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintFill : MonoBehaviour, IPaintFill, IEntityComponent
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Image _image;
        private float _z;

        public Vector2 RectPosition => _transform.anchoredPosition;

        public void Register(IEntityBuilder builder)
        {
            _z = _transform.position.z;
            
            builder.RegisterComponent(this)
                .As<IPaintFill>();
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public void SetMaterial(Material material)
        {
            _image.material = material;
        }

        public void ResetMaterial()
        {
            _image.material = null;
        }

        public void SetWorldPosition(Vector2 position)
        {
            _transform.position = new Vector3(position.x, position.y, _z);
        }

        public void SetRectPosition(Vector2 position)
        {
            _transform.anchoredPosition = position;
        }

        public void SetSize(float size)
        {
            _transform.sizeDelta = new Vector2(size, size);
        }
    }
}