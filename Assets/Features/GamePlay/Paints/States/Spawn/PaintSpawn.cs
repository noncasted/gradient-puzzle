using System;
using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;
using Global.Systems;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintSpawn : IPaintSpawn, IState
    {
        public PaintSpawn(
            IUpdater updater,
            IStateMachine stateMachine,
            IPaintTransform transform,
            IPaintImage image,
            PaintSpawnOptions options,
            PaintSpawnDefinition definition)
        {
            _updater = updater;
            _stateMachine = stateMachine;
            _transform = transform;
            _image = image;
            _options = options;
            Definition = definition;
        }

        private readonly IUpdater _updater;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintTransform _transform;
        private readonly IPaintImage _image;
        private readonly PaintSpawnOptions _options;

        public IStateDefinition Definition { get; }

        public async UniTask Process(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);

            switch (target)
            {
                case IPaintDock dock:
                {
                    _transform.AttachTo(dock.Transform);
                    _transform.SetPosition(Vector2.zero);
                    var dockSize = dock.Size;
                    _image.SetSize(_options.StartDockSize);

                    await _updater.CurveProgression(handle.Lifetime, _options.DockScaleCurve, progress =>
                    {
                        _image.SetSize(dockSize * 2 * progress);
                    });

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