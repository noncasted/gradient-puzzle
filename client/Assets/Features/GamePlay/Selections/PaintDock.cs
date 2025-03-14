using System.Collections.Generic;
using GamePlay.Common;
using Global.UI;
using Internal;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        [SerializeField] private AreaCenter _center;
        [SerializeField] private MaskableGraphic _image;

        private readonly ViewableProperty<bool> _isTouched = new(false);

        private float _size;
        private AreaCenter[] _centers;
        private RenderMaskData _maskData;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public Vector2 Position => _selfTransform.anchoredPosition;
        public RectTransform SelfTransform => _paintRoot;
        public RectTransform RootCenter => _paintRoot;
        public RenderMaskData MaskData => _maskData;
        public IPaintHandle PaintHandle { get; } = new PaintHandle();
        public IReadOnlyList<AreaCenter> Centers => _centers;

        public float Size => _size;

        public void UpdateTransform(int areaSize)
        {
            var halfSize = areaSize / 2;
            _selfTransform.SetAnchor(AnchorPresets.MiddleCenter, -halfSize, halfSize);
        }

        public void Construct(float size, RenderMaskData maskData)
        {
            _maskData = maskData;
            _size = size;
            _centers = new[] { _center };
            _image.material = maskData.Area;
        }

        public bool IsInside(Vector2 position)
        {
            var rect = _selfTransform.rect;
            return rect.Contains(position);
        }

        public float GetMinDistanceToBorder(Vector2 position)
        {
            var magnitude = position.magnitude;
            var rect = _selfTransform.rect;

            var halfWidth = rect.width / 2;
            var distance = halfWidth - magnitude;

            if (distance < 0)
                return 0;

            return distance;
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