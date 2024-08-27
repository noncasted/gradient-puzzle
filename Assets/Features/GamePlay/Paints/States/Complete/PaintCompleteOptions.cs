using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintCompleteOptions : EnvAsset
    {
        [SerializeField] private Curve _completionCurve;
        [SerializeField] private int _completionColorAdjustment;

        public Curve CompletionCurve => _completionCurve;
        public float CompletionColorAdjustment => _completionColorAdjustment;
    }
}