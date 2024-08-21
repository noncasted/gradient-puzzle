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
        [SerializeField] private RectTransform _areaTransform;
        
        private Vector2Int _textureSize;
        private Vector2 _rectSize;
        private Vector2 _rectPivot;
        private Vector2Int _position;

        public Color Color => _color;
        public Vector2Int Position => _position;
        
        public void Construct(Texture2D source)
        {
            _textureSize = new Vector2Int(source.width, source.height);
            _rectSize = _areaTransform.rect.size;
            _rectPivot = _areaTransform.pivot;

            var selfPosition = _selfTransform.anchoredPosition;
            
            var textureX = (selfPosition.x / _rectSize.x) + _rectPivot.x;
            var textureY = (selfPosition.y / _rectSize.y) + _rectPivot.y;

            var x = Mathf.FloorToInt(textureX * _textureSize.x);
            var y = Mathf.FloorToInt(textureY * _textureSize.y);
            
            _position = new Vector2Int(x, y);
        }

        private void OnValidate()
        {
            _image.color = _color;
        }
    }
}