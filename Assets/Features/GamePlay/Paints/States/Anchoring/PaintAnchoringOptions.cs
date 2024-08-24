using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintAnchoringOptions : EnvAsset
    {
        [SerializeField] [Min(0f)] private float _moveSpeed;
        [SerializeField] [Min(0f)] private float _dragSize;
        [SerializeField] private Curve _dragSizeCurve;
        [SerializeField] private float _distanceThreshold = 1;

        public float MoveSpeed => _moveSpeed;
        public float DragSize => _dragSize;
        public Curve DragSizeCurve => _dragSizeCurve;
        public float DistanceThreshold => _distanceThreshold;
    }
}