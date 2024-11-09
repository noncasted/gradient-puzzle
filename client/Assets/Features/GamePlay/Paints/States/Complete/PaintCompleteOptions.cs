using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    public class PaintCompleteOptions : EnvAsset
    {
        [SerializeField] private Curve _completionCurve;
        [SerializeField] private int _completionColorAdjustment;

        public Curve CompletionCurve => _completionCurve;
        public float CompletionColorAdjustment => _completionColorAdjustment;
    }
}