using MPUIKIT;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class LevelConstructorColorPoint : MonoBehaviour
    {
        [SerializeField] private Color _color;
        [SerializeField] private MPImage _image;
        [SerializeField] private RectTransform _selfTransform;
        
        public Color Color => _color;
        public Vector2 Position => _selfTransform.anchoredPosition;
        
        private void OnValidate()
        {
            _image.color = _color;
        }
    }
}