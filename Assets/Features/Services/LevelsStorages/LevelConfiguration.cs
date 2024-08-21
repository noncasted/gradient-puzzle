using Features.GamePlay;
using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Services
{
    [InlineEditor]
    public class LevelConfiguration : EnvAsset, ILevelConfiguration
    {
        [SerializeField] private Level _prefab;

        public Level Prefab => _prefab;
    }
}