using System.Collections.Generic;
using System.Linq;
using Features.Services.Inputs;
using Global.Systems;
using Internal;
using UnityEngine;
using VContainer;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class Level : MonoBehaviour, ILevel
    {
        [SerializeField] private Area[] _areas;

        private IUpdater _updater;
        private IGameInput _input;

        public IReadOnlyList<IArea> Areas => _areas;

        [Inject]
        private void Construct(IUpdater updater, IGameInput input)
        {
            _input = input;
            _updater = updater;
        }

        private void Awake()
        {
            _areas = _areas.OrderByDescending(t => t.Position.y).ToArray();

            for (var i = 0; i < _areas.Length; i++)
            {
                var area = _areas[i];
                area.transform.parent = transform;
                area.transform.SetSiblingIndex(i);
            }
        }

        public void Construct(Area[] areas)
        {
            _areas = areas;
        }

        public void Setup(IReadOnlyLifetime lifetime)
        {
            _updater.RunUpdateAction(lifetime, _ =>
            {
                foreach (var area in _areas)
                    area.CheckTouch(_input.CursorPosition);
            });
        }
    }
}