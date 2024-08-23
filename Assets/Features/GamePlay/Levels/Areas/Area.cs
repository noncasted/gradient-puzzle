using System;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class Area : MonoBehaviour, IArea
    {
        [SerializeField] private bool _isStartingPoint;
        [SerializeField] private RectTransform _transform;
        [SerializeField] private AreaRenderer _renderer;
        [SerializeField] private AreaData _data;

        private readonly ViewableProperty<bool> _isTouched = new();

        private IPaint _paint;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public IPaint Paint => _paint;
        public Vector2 Position => _data.Center;
        public RectTransform Transform => _transform;
        public bool IsStartingPoint => _isStartingPoint;

        public AreaData Data => _data;
        public Color Source => _data.Color;
        public AreaRenderer Renderer => _renderer;

        public void Construct(AreaData data)
        {
            _data = data;
            _renderer.color = data.Color;
        }

        public void Setup(Color color)
        {
            _renderer.color = color;
        }

        public void CheckTouch(Vector2 cursorPosition)
        {
            var isInside = _data.IsInside(cursorPosition);
            Debug.Log(isInside);
            _isTouched.Set(isInside);
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