using System;
using NaughtyAttributes;
using UnityEngine;

namespace Internal
{
    public interface ICurve
    {
        float Time { get; }
        AnimationCurve Animation { get; }
        
        CurveInstance CreateInstance();
    }

    [Serializable]
    public class Curve : ICurve
    {
        public Curve(float time, AnimationCurve curve)
        {
            _time = time;
            _curve = curve;
        }

        [SerializeField] [Min(0f)] private float _time;
        [SerializeField] [CurveRange] private AnimationCurve _curve;

        public float Time => _time;
        public AnimationCurve Animation => _curve;

        public CurveInstance CreateInstance()
        {
            return new CurveInstance(this);
        }
    }

    public struct CurveInstance
    {
        public CurveInstance(Curve curve)
        {
            _curve = curve;
            _progress = 0f;
        }

        private readonly Curve _curve;

        private float _progress;
        
        public bool IsFinished => _progress >= 1f;

        public float Step(float delta)
        {
            _progress += delta / _curve.Time;

            if (_progress > 1f)
                _progress = 1f;

            return _curve.Evaluate(_progress);
        }
    }

    public static class CurveExtensions
    {
        public static float Evaluate(this ICurve curve, float progress)
        {
            return curve.Animation.Evaluate(progress);
        }
    }
}