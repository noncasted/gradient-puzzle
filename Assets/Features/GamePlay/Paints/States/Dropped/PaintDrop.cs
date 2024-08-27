using System;
using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;
using Global.Systems;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintDrop : IPaintDrop, IState
    {
        public PaintDrop(
            IUpdater updater,
            IStateMachine stateMachine,
            IPaintTransform transform,
            IPaintImage image,
            PaintDropOptions options,
            PaintDropDefinition definition)
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
        private readonly PaintDropOptions _options;

        public IStateDefinition Definition { get; }

        public void Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);

            Process(handle.Lifetime, target).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime, IPaintTarget target)
        {
            switch (target)
            {
                case IPaintDock dock:
                {
                    _transform.AttachTo(dock.Transform);
                    _transform.SetPosition(Vector2.zero);
                    var dockSize = dock.Size;
                    _image.ResetMaterial();

                    await _updater.CurveProgression(lifetime, _options.DockScaleCurve,
                        progress => { _image.SetSize(dockSize * 2 * progress); });
                    
                    break;
                }
                case IArea area:
                {
                    _transform.AttachTo(area.Transform);
                    _image.SetMaterial(area.MaskData.Content);

                    await _updater.CurveProgression(lifetime, _options.AreaScaleCurve,
                        progress => { _image.SetSize(PaintExtensions.MaxRadius * progress); });
                    
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