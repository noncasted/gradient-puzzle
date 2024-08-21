using System;
using Internal;
using MPUIKIT;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class Area : MonoBehaviour, IArea
    {
        [SerializeField] private bool _isStartingPoint;
        [SerializeField] private MPImage _image;
        [SerializeField] private RectTransform _transform;
        [SerializeField] [ReadOnly] private Vector2 _center;

        private readonly ViewableProperty<bool> _isTouched = new();

        private Color32[] _pixels;
        private Vector2Int _textureSize;
        private Vector2 _rectSize;
        private Vector2 _rectPivot;
        private Color _source;
        private IPaint _paint;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public IPaint Paint => _paint;
        public Vector2 Position => _center;
        public RectTransform Transform => _transform;
        public Color Source => _source;
        public bool IsStartingPoint => _isStartingPoint;
        public MPImage Image => _image;

        public void Construct(Vector2 center)
        {
            _center = center - _transform.sizeDelta / 2f;
        }

        public void Setup(Color color)
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

            var index = y * _textureSize.x + x;

            if (index < 0 || index >= _pixels.Length)
            {
                _isTouched.Set(false);
                return;
            }

            var pixelColor = _pixels[index];

            var isTouched = pixelColor.a > 0.1f;
            _isTouched.Set(isTouched);
        }

        public void SetPaint(IPaint paint)
        {
            _paint = paint;
        }

        public void RemovePaint(IPaint paint)
        {
            if (_paint != paint)
                throw new Exception();
            
            _paint = null;
        }
    }
}