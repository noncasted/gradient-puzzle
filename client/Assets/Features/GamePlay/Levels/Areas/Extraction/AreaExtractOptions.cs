using UnityEngine;

namespace GamePlay.Levels
{
    public readonly struct AreaExtractOptions
    {
        public AreaExtractOptions(
            Vector2 rectSize,
            byte colorEpsilon,
            int simplifyIterations,
            float distanceThreshold,
            int erosionPixels)
        {
            RectSize = rectSize;
            ColorEpsilon = colorEpsilon;
            SimplifyIterations = simplifyIterations;
            DistanceThreshold = distanceThreshold;
            ErosionPixels = erosionPixels;
        }

        public Vector2 RectSize { get; }
        public byte ColorEpsilon { get; }
        public int SimplifyIterations { get; }
        public float DistanceThreshold { get; }
        public int ErosionPixels { get; }
    }

    public readonly struct AreaExtractedColor
    {
        public AreaExtractedColor(Color32 value, byte epsilon)
        {
            _value = value;
            _epsilon = epsilon;
        }
        
        private readonly Color32 _value;
        private readonly byte _epsilon;
        
        public bool IsEqual(Color32 color)
        {
            var areEqual = Mathf.Abs(_value.r - color.r) <= _epsilon
                           && Mathf.Abs(_value.g - color.g) <= _epsilon
                           && Mathf.Abs(_value.b - color.b) <= _epsilon
                           && color.a > 100;
            
            return areEqual;
        }
    }
}