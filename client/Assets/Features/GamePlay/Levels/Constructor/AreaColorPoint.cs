using MPUIKIT;
using UnityEngine;

namespace GamePlay.Levels
{
    [DisallowMultipleComponent]
    public class AreaColorPoint : MonoBehaviour
    {
        [SerializeField] private MPImage _image;
        [SerializeField] private RectTransform _selfTransform;

        public Color Color => _image.color;
        public Vector2 Position => _selfTransform.position;

        public void SetColor(Color color)
        {
            _image.color = color;
        }
    }
}