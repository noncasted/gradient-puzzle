using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintReturnOptions : EnvAsset
    {
        [SerializeField] private Curve _curve;

        public Curve Curve => _curve;
    }
}