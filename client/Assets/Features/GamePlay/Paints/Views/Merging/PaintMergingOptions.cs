using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Paints
{
    [InlineEditor]
    public partial class PaintMergingOptions : ScriptableObject
    {
        [SerializeField] private float _initTime = 0.5f;
        [SerializeField] private float _insideScaleTime = 1f;
        [SerializeField] private float _minFillSize = 25;
        [SerializeField] private float _startDistance;
        [SerializeField] private float _step;
        [SerializeField] private float _time = 0.5f;
        [SerializeField] [CurveRange(0, -1, 1, 1)] private AnimationCurve _middlePointHeightCurve;
        [SerializeField] [CurveRange] private AnimationCurve _middlePointPositionCurve;
        [SerializeField] [CurveRange] private AnimationCurve _targetPositionCurve;
        [SerializeField] [CurveRange] private AnimationCurve _targetSizeCurve;

        public float InitTime => _initTime;
        public float InsideScaleTime => _insideScaleTime;
        public float MinFillSize => _minFillSize;
        public float StartDistance => _startDistance;
        public float Step => _step;
        public float Time => _time;
        public AnimationCurve MiddlePointHeightCurve => _middlePointHeightCurve;
        public AnimationCurve MiddlePointPositionCurve => _middlePointPositionCurve;
        public AnimationCurve TargetPositionCurve => _targetPositionCurve;
        public AnimationCurve TargetSizeCurve => _targetSizeCurve;
    }
}