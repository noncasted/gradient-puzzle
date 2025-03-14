using System.Collections.Generic;
using System.Linq;
using Global.Systems;
using Internal;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace GamePlay.Levels
{
    [DisallowMultipleComponent]
    public class Level : MonoBehaviour, ILevel
    {
        [SerializeField] private Area[] _areas;
        [SerializeField] private RectTransform _transform;

        private IUpdater _updater;
        private IGameInput _input;

        public IReadOnlyList<IArea> Areas => _areas;
        public RectTransform Transform => _transform;
        
        private static readonly ViewableDelegate<Level> _constructorRequest = new();

        public static IViewableDelegate<Level> ConstructorRequest => _constructorRequest;

        [Inject]
        private void Construct(IUpdater updater, IGameInput input)
        {
            _input = input;
            _updater = updater;
        }

        private void Awake()
        {
            _areas = GetComponentsInChildren<Area>(true).Reverse().ToArray();
        }

        public void Construct(Area[] areas)
        {
            _areas = areas;
        }

        public void Setup(IReadOnlyLifetime lifetime)
        {
            _updater.RunUpdateAction(lifetime, _ =>
            {
                var isInside = false;

                foreach (var area in _areas)
                {
                    if (isInside == false)
                        isInside = area.CheckTouch(_input.CursorPosition);
                    else
                        area.ResetTouch();
                }
            });
        }

        [Button("Open Constructor")]
        public void OpenLevelConstructor()
        {
            _constructorRequest.Invoke(this);
        }
    }
}