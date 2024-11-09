using Internal;
using UnityEngine;

namespace GamePlay.Levels
{
    public class AreaColorGroupOptions : EnvAsset
    {
        [SerializeField] private AreaColorGroup _groupPrefab;

        public AreaColorGroup GroupPrefab => _groupPrefab;
    }
}