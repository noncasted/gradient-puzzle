using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintSelectionOptions : EnvAsset
    {
        [SerializeField] private PaintDock _prefab;

        public PaintDock Prefab => _prefab;
    }
}