using System.Collections.Generic;
using GamePlay.Common;
using Internal;
using Services;
using UnityEngine;

namespace GamePlay.Levels
{
    [DisallowMultipleComponent]
    public class Area : MonoBehaviour, IArea
    {
        [SerializeField] private bool _isAnchor;
        [SerializeField] private RectTransform _transform;
        [SerializeField] private AreaCompoundRenderer _renderer;
        [SerializeField] private List<AreaData> _datas;
        [SerializeField] private Vector2 _center;
        [SerializeField] private RectTransform _centerTransform = new();

        private readonly ViewableProperty<bool> _isTouched = new();
        private readonly ViewableProperty<bool> _isCompleted = new();

        private Color _color;
        private RenderMaskData _maskData;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public IViewableProperty<bool> IsCompleted => _isCompleted;
        public Vector2 Position => _centerTransform.position;
        public RectTransform Transform => _transform;
        public RectTransform CenterTransform => _centerTransform;
        public bool IsAnchor => _isAnchor;
        public RenderMaskData MaskData => _maskData;
        public IPaintHandle PaintHandle { get; } = new PaintHandle();
        public Color Source => _color;
        public AreaCompoundRenderer Renderer => _renderer;
        public IReadOnlyList<AreaData> Datas => _datas;

        public void Construct(IReadOnlyList<AreaData> datas)
        {
            _datas = new List<AreaData>(datas);
            _renderer.Construct(datas);
            _center = AreaDataExtensions.GetCenter(datas);
            _centerTransform.anchoredPosition = _center;
        }

        public void Setup(Color color, RenderMaskData maskData, Transform outlineParent)
        {
            _centerTransform.SetAsLastSibling();
            _color = _renderer.Color;
            _maskData = maskData;
            _renderer.SetMaterial(maskData.Area);
            _renderer.SetColor(color);

            var lifetime = this.GetObjectLifetime();

            foreach (var areaRenderer in _renderer.Renderers)
            {
                areaRenderer.Outline.transform.parent = outlineParent;
                areaRenderer.Outline.transform.SetAsLastSibling();
            }

            PaintHandle.Paint.Advise(lifetime, paint =>
            {
                if (paint == null)
                {
                    foreach (var areaRenderer in _renderer.Renderers)
                        areaRenderer.Outline.Enable();

                    _isCompleted.Set(false);
                    return;
                }

                foreach (var areaRenderer in _renderer.Renderers)
                    areaRenderer.Outline.Disable();

                _isCompleted.Set(paint.Color == _color);
            });
        }

        public bool IsInside(Vector2 position)
        {
            return CheckInside(position);
        }

        public float GetMinDistanceToBorder(Vector2 position)
        {
            var minDistance = float.MaxValue;

            foreach (var data in _datas)
            {
                var distance = data.GetMinDistanceToBorder(position);

                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;
        }

        public void CheckTouch(Vector2 cursorPosition)
        {
            if (_isAnchor == true)
                return;

            _isTouched.Set(CheckInside(cursorPosition));
        }

        private bool CheckInside(Vector2 position)
        {
            foreach (var data in _datas)
            {
                if (data.IsInside(position) == false)
                    continue;

                return true;
            }

            return false;
        }
    }
}