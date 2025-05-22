using System.Collections.Generic;
using GamePlay.Common;
using GamePlay.Levels;
using Global.UI;
using Internal;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay.Selections
{
    [DisallowMultipleComponent]
    public class PaintDock :
        MonoBehaviour,
        IPaintDock,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private RectTransform _selfTransform;
        [SerializeField] private RectTransform _paintRoot;
        [SerializeField] private AreaShapeRenderer _image;
        [SerializeField] private AreaShapeData _shape;
        [SerializeField] private float _baseSize = 170f;

        private readonly ViewableProperty<bool> _isTouched = new(false);

        [SerializeField] private List<Vector2> _innerPoints = new();
        [SerializeField] private List<Vector2> _systemPoints = new();
        private float _size;
        private RenderMaskData _maskData;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public Vector2 Position => _selfTransform.anchoredPosition;
        public RectTransform SelfTransform => _paintRoot;
        public RectTransform RootCenter => _paintRoot;
        public RenderMaskData MaskData => _maskData;
        public IPaintHandle PaintHandle { get; } = new PaintHandle();
        public IReadOnlyList<Vector2> InnerPoints => _innerPoints;

        public float Size => _size;

        public void UpdateTransform(int areaSize)
        {
            var halfSize = areaSize / 2;
            _selfTransform.SetAnchor(AnchorPresets.MiddleCenter, -halfSize, halfSize);
            
            _innerPoints.Clear();
            _systemPoints.Clear();
            
            var scale = _selfTransform.sizeDelta.x / _baseSize;
            
            foreach (var point in _shape.InnerPoint)
                _innerPoints.Add(point * scale + _selfTransform.anchoredPosition);
            
            foreach (var point in _shape.SystemPoints)
                _systemPoints.Add(point * scale + _selfTransform.anchoredPosition);
            
            var renderPoints = new List<Vector2>(_shape.RenderPoints);
            
            for (var i = 0; i < renderPoints.Count; i++)
                renderPoints[i] *= scale;
            
            _image.SetPoints(renderPoints);
        }

        public void Construct(float size, RenderMaskData maskData)
        {
            _maskData = maskData;
            _size = size;
            _image.material = maskData.Area;
        }

        public bool IsInside(Vector2 position)
        {
            return _systemPoints.IsInside(position);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isTouched.Set(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isTouched.Set(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isTouched.Set(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isTouched.Set(false);
        }
    }
}