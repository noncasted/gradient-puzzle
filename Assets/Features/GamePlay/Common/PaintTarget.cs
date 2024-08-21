using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.GamePlay
{
    public class PaintTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Pointer enter {gameObject.name}");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }
    }
}