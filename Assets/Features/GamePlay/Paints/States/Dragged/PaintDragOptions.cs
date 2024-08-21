using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintDragOptions : EnvAsset
    {
        [SerializeField] [Min(0f)] private float _moveSpeed;
        [SerializeField] [Min(0f)] private float _dragSize;
        [SerializeField] private Curve _dragSizeCurve;

        public float MoveSpeed => _moveSpeed;
        public float DragSize => _dragSize;
        public Curve DragSizeCurve => _dragSizeCurve;
    }
}