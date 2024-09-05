using Internal;
using NaughtyAttributes;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintMoverOptions : EnvAsset
    {
        [SerializeField] [Min(0f)] private float _moveSpeed;
        [SerializeField] [Min(0f)] private float _moveSize;

        [SerializeField] private Curve _dockScaleCurve;
        [SerializeField] private Curve _areaScaleCurve;

        [SerializeField] [Min(0f)] private float _transitHeight;
        [SerializeField] [CurveRange(0f, 0f, 10f, 10f)] private AnimationCurve _transitTimeCurve;
        [SerializeField] [CurveRange] private AnimationCurve _transitMoveCurve;
        [SerializeField] [CurveRange] private AnimationCurve _transitHeightCurve;

        public float MoveSize => _moveSize;
        public float MoveSpeed => _moveSpeed;
        public Curve DockScaleCurve => _dockScaleCurve;
        public Curve AreaScaleCurve => _areaScaleCurve;
        public AnimationCurve TransitMoveCurve => _transitMoveCurve;
        public AnimationCurve TransitHeightCurve => _transitHeightCurve;
        public float TransitHeight => _transitHeight;
        public AnimationCurve TransitTimeCurve => _transitTimeCurve;
    }
}