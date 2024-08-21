using System;
using Global.UI;
using Internal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class PaintDock : MonoBehaviour, IPaintDock, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _transform;
        
        private readonly ViewableProperty<bool> _isTouched = new(false);
        
        private float _size;
        private IPaint _paint;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public IPaint Paint => _paint;
        public Vector2 Position => _transform.anchoredPosition;
        public RectTransform Transform => _transform;
        public float Size => _size;
        
        public void UpdateTransform(int areaSize)
        {
            var halfSize = areaSize / 2;
            _transform.SetAnchor(AnchorPresets.MiddleCenter, -halfSize, halfSize);
        }

        public void Construct(float size)
        {
            _size = size;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isTouched.Set(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isTouched.Set(false);
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