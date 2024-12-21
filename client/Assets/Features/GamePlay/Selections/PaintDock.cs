using GamePlay.Common;
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

        private readonly ViewableProperty<bool> _isTouched = new(false);

        private float _size;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public Vector2 Position => _selfTransform.anchoredPosition;
        public RectTransform Transform => _paintRoot;
        public RectTransform CenterTransform => _paintRoot;
        public RenderMaskData MaskData => null;
        public IPaintHandle PaintHandle { get; } = new PaintHandle();

        public float Size => _size;

        public void UpdateTransform(int areaSize)
        {
            var halfSize = areaSize / 2;
            _selfTransform.SetAnchor(AnchorPresets.MiddleCenter, -halfSize, halfSize);
        }

        public void Construct(float size)
        {
            _size = size;
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