using System;
using Common.StateMachines;
using Cysharp.Threading.Tasks;
using GamePlay.Common;
using GamePlay.Levels;
using GamePlay.Selections;
using Global.Systems;
using UnityEngine;

namespace GamePlay.Paints
{
    public class PaintSpawn : IPaintSpawn, IState
    {
        public PaintSpawn(
            IUpdater updater,
            IStateMachine stateMachine,
            IPaintInterceptor interceptor,
            IPaintTransform transform,
            IPaintImage image,
            IPaintMerging merging,
            PaintSpawnOptions options,
            PaintSpawnDefinition definition)
        {
            _updater = updater;
            _stateMachine = stateMachine;
            _interceptor = interceptor;
            _transform = transform;
            _image = image;
            _merging = merging;
            _options = options;
            Definition = definition;
        }

        private readonly IUpdater _updater;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintInterceptor _interceptor;
        private readonly IPaintTransform _transform;
        private readonly IPaintImage _image;
        private readonly IPaintMerging _merging;
        private readonly PaintSpawnOptions _options;

        public IStateDefinition Definition { get; }

        public async UniTask Process(IPaintTarget target)
        {
            _interceptor.Attach(target);
            var handle = _stateMachine.CreateHandle(this);

            switch (target)
            {
                case IPaintDock dock:
                {
                    _transform.AttachTo(dock.SelfTransform);
                    _transform.SetRectPosition(Vector2.zero);
                    _merging.Show(handle.Lifetime, target);

                    break;
                }
                case IArea area:
                {
                    break;
                }
                default:
                {
                    throw new Exception();
                }
            }
        }
    }
}