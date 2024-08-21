using Internal;
using MPUIKIT;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class Area : MonoBehaviour, IArea
    {
        [SerializeField] private MPImage _image;
        [SerializeField] private RectTransform _transform;

        private Color32[] _pixels;
        private Vector2Int _textureSize;
        private Vector2 _rectSize;
        private Vector2 _rectPivot;

        private Color _source;

        private readonly ViewableProperty<bool> _isTouched = new();

        public IViewableProperty<bool> IsTouched => _isTouched;
        public Color Source => _source;
        public MPImage Image => _image;

        public void Construct(Color color)
        {
            _source = _image.color;
            _image.color = color;
            
            var texture = _image.sprite.texture;
            _pixels = texture.GetPixels32();
            _textureSize = new Vector2Int(texture.width, texture.height);
            _rectSize = _transform.rect.size;
            _rectPivot = _transform.pivot;
        }

        public void CheckTouch(Vector2 cursorPosition)
        {
            var textureX = (cursorPosition.x / _rectSize.x) + _rectPivot.x;
            var textureY = (cursorPosition.y / _rectSize.y) + _rectPivot.y;

            var x = Mathf.FloorToInt(textureX * _textureSize.x);
            var y = Mathf.FloorToInt(textureY * _textureSize.y);

            var pixelColor = _pixels[y * _textureSize.x + x];

            var isTouched = pixelColor.a > 0.1f;
            _isTouched.Set(isTouched);
        }
    }
}