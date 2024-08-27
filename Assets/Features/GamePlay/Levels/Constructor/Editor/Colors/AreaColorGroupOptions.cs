using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class AreaColorGroupOptions : EnvAsset
    {
        [SerializeField] private AreaColorGroup _groupPrefab;

        public AreaColorGroup GroupPrefab => _groupPrefab;
    }
}