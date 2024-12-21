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

        public async UniTask Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            
            switch (target)
            {
                case IPaintDock dock:
                {
                    _transform.AttachTo(dock.Transform);
                    _transform.SetRectPosition(Vector2.zero);
                    var dockSize = dock.Size;
                    _image.ResetMaterial();

                    await _updater.CurveProgression(handle.Lifetime, _options.DockScaleCurve,
                        progress => { _image.SetSize(dockSize * 2 * progress); });
                    
                    _transform.SetRectPosition(dock.CenterTransform.anchoredPosition);
                    break;
                }
                case IArea area:
                {
                    _transform.AttachTo(area.Transform);
                    _image.SetMaterial(area.MaskData.Content);

                    await _updater.CurveProgression(handle.Lifetime, _options.AreaScaleCurve,
                        progress =>
                        {
                            _image.SetSize(PaintExtensions.MaxRadius * progress);
                        });
                    
                    _transform.SetRectPosition(area.CenterTransform.anchoredPosition);
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