using Internal;
using NaughtyAttributes;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintSpawnOptions : EnvAsset
    {
        [SerializeField] [Min(0f)] private float _startDockSize;
        [SerializeField] [Min(0f)] private float _dockScaleTime;

        [SerializeField] [CurveRange(0f, 0f, 1f, 1.3f)]
        private AnimationCurve _dockScaleCurve;

        [SerializeField] private float _areaScaleTime;

        public float StartDockSize => _startDockSize;
        public Curve DockScaleCurve => new(_dockScaleTime, _dockScaleCurve);
    }
}