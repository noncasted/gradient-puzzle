using Internal;
using UnityEngine;

namespace GamePlay.Levels
{
    public class LevelLoaderOptions : EnvAsset
    {
        [SerializeField] private Gradient _areasGradient;
        
        public Gradient AreasGradient => _areasGradient;
    }
}