using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class LevelConstructorOptions : EnvAsset
    {
        [SerializeField] private Gradient _areasGradient;
        
        public Gradient AreasGradient => _areasGradient;
    }
}