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

        public IViewableProperty<bool> IsTouched => _isTouched;
        public RectTransform Transform => _transform;
        public float Size => _size;

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
    }
}