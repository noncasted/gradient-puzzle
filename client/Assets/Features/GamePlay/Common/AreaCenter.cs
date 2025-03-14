using UnityEngine;

namespace GamePlay.Common
{
    public class AreaCenter : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private float _size;
        
        public RectTransform Transform => _transform;
        public float Size => _size;

        public void Setup(Vector2 position, float size)
        {
            _transform.anchoredPosition = position;
            _size = size;
        }
    }
}