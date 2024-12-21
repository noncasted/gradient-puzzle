using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Levels
{
    [InlineEditor]
    public class LevelConstructorOptions : EnvAsset
    {
        [SerializeField] private int _simplifyIterations = 1;
        [SerializeField] private float _distanceThreshold = 1f;
        [SerializeField] private int _erosionPixels;
        [SerializeField] private byte _colorEpsilon = 10;
        [SerializeField] private Area _prefab;

        public int SimplifyIterations => _simplifyIterations;
        public float DistanceThreshold => _distanceThreshold;
        public int ErosionPixels => _erosionPixels;
        public byte ColorEpsilon => _colorEpsilon;
        public Area Prefab => _prefab;
    }
}