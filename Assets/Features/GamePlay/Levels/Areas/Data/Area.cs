using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.Services.RenderOptions;
using Global.Systems;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class Area : MonoBehaviour, IArea
    {
        [SerializeField] private bool _isAnchor;
        [SerializeField] private RectTransform _transform;
        [SerializeField] private AreaCompoundRenderer _renderer;
        [SerializeField] private List<AreaData> _datas;
        [SerializeField] private Vector2 _center;

        private readonly ViewableProperty<bool> _isTouched = new();
        private readonly ViewableProperty<bool> _isCompleted = new();

        private Color _color;
        private IPaint _paint;
        private RenderMaskData _maskData;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public IViewableProperty<bool> IsCompleted => _isCompleted;
        public IPaint Paint => _paint;
        public Vector2 Position => _center;
        public RectTransform Transform => _transform;
        public bool IsAnchor => _isAnchor;
        public RenderMaskData MaskData => _maskData;
        public Color Source => _color;
        public AreaCompoundRenderer Renderer => _renderer;
        public IReadOnlyList<AreaData> Datas => _datas;

        public void Construct(IReadOnlyList<AreaData> datas)
        {
            _datas = new List<AreaData>(datas);
            _renderer.Construct(datas);
            _center = AreaDataExtensions.GetCenter(datas);
        }

        public void Setup(Color color, RenderMaskData maskData)
        {
            _color = _renderer.Color;
            _maskData = maskData;
            _renderer.SetMaterial(maskData.Area);
            _renderer.SetColor(color);
        }

        public void CheckTouch(Vector2 cursorPosition)
        {
            if (_isAnchor == true)
                return;

            _isTouched.Set(CheckInside());

            return;

            bool CheckInside()
            {
                foreach (var data in _datas)
                {
                    if (data.IsInside(cursorPosition) == false)
                        continue;

                    return true;
                }

                return false;
            }
        }

        public void SetPaint(IPaint paint)
        {
            _paint = paint;

            if (_paint.Color == _color)
                _isCompleted.Set(true);
            else
                _isCompleted.Set(false);
        }

        public void RemovePaint(IPaint paint)
        {
            if (_paint != paint)
                throw new Exception();

            _paint = null;
            _isCompleted.Set(false);
        }
    }
}