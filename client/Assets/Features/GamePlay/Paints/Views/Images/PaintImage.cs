using Internal;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintImage : MonoBehaviour, IPaintImage, IEntityComponent
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _transform;
        
        private Material _defaultMaterial;

        public float Size => _transform.rect.size.x;
        public Color Color => _image.color;

        public void Register(IEntityBuilder builder)
        {
            _defaultMaterial = _image.material;
            builder.RegisterComponent(this)
                .As<IPaintImage>();
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public void SetSize(float size)
        {
            _transform.sizeDelta = new Vector2(size, size);
        }

        public void SetMaterial(Material material)
        {
            _image.material = material;
        }

        public void ResetMaterial()
        {
            _image.material = _defaultMaterial;
        }
    }
}