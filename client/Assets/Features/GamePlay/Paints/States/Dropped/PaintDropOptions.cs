using Internal;
using NaughtyAttributes;
using UnityEngine;

namespace GamePlay.Paints
{
    public class PaintDropOptions : EnvAsset
    {
        [SerializeField] [CurveRange(1.3f)] private AnimationCurve _dockScaleCurve;
        [SerializeField] private float _dockScaleTime;

        [SerializeField] [CurveRange] private AnimationCurve _areaScaleCurve;
        [SerializeField] private float _areaScaleTime;

        public Curve DockScaleCurve => new(_dockScaleTime, _dockScaleCurve);
        public Curve AreaScaleCurve => new(_areaScaleTime, _areaScaleCurve);
    }
}