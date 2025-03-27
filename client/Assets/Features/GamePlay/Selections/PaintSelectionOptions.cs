using Internal;
using UnityEngine;

namespace GamePlay.Selections
{
    public class PaintSelectionOptions : EnvAsset
    {
        [SerializeField] private PaintDock _prefab;

        public PaintDock Prefab => _prefab;
    }
}