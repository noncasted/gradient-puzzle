using System.Collections.Generic;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class Level : MonoBehaviour, ILevel
    {
        [SerializeField] private Area[] _areas;

        public IReadOnlyList<IArea> Areas => _areas;

        public void Construct(Area[] areas)
        {
            _areas = areas;
        }
    }
}