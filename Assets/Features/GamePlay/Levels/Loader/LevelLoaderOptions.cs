using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class LevelLoaderOptions : EnvAsset
    {
        [SerializeField] private Gradient _areasGradient;
        
        public Gradient AreasGradient => _areasGradient;
    }
}