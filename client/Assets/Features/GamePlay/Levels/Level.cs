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

        private IUpdater _updater;
        private IGameInput _input;

        public IReadOnlyList<IArea> Areas => _areas;  
        
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
            // _areas = _areas.OrderByDescending(t => t.Position.y).ToArray();
            //
            // for (var i = 0; i < _areas.Length; i++)
            // {
            //     var area = _areas[i];
            //     area.transform.parent = transform;
            //     area.transform.SetSiblingIndex(i);
            // }
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
        
        [Button("Open Constructor")]
        public void OpenLevelConstructor()
        {
            _constructorRequest.Invoke(this);
        }
    }
}