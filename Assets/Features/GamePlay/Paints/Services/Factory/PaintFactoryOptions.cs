using Internal;
using UnityEngine;

namespace Features.GamePlay.Paints
{
    public class PaintFactoryOptions : EnvAsset
    {
        [SerializeField] private PaintView _prefab;
        
        public PaintView Prefab => _prefab;
    }
}