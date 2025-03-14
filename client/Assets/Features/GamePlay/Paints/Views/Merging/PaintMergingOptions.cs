using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Paints
{
    [InlineEditor]
    public class PaintMergingOptions : ScriptableObject
    {
        [SerializeField] private float _startDistance;
        [SerializeField] private float _step;
        [SerializeField] [CurveRange(0, -1, 1, 1)] private AnimationCurve _middlePointHeightCurve;
        [SerializeField] [CurveRange] private AnimationCurve _middlePointPositionCurve;
        [SerializeField] [CurveRange] private AnimationCurve _targetPositionCurve;
        [SerializeField] [CurveRange] private AnimationCurve _targetSizeCurve;

        public float StartDistance => _startDistance;
        public float Step => _step;
        public AnimationCurve MiddlePointHeightCurve => _middlePointHeightCurve;
        public AnimationCurve MiddlePointPositionCurve => _middlePointPositionCurve;
        public AnimationCurve TargetPositionCurve => _targetPositionCurve;
        public AnimationCurve TargetSizeCurve => _targetSizeCurve;
    }
}