using System;
using System.Collections.Generic;
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
        [SerializeField] private GameObject _colors;

        private IUpdater _updater;
        private IGameInput _input;

        public IReadOnlyList<IArea> Areas => _areas;
        public IReadOnlyList<Area> AreasInternal => _areas;

        [Inject]
        private void Construct(IUpdater updater, IGameInput input)
        {
            _input = input;
            _updater = updater;
        }

        public void Construct(Area[] areas)
        {
            _areas = areas;
        }

        private void Awake()
        {
            _colors.SetActive(false);
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