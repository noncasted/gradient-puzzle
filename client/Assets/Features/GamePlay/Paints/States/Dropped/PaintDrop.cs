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
            IPaintMerging merging,
            PaintDropOptions options,
            PaintDropDefinition definition)
        {
            _updater = updater;
            _stateMachine = stateMachine;
            _transform = transform;
            _image = image;
            _merging = merging;
            _options = options;
            Definition = definition;
        }

        private readonly IUpdater _updater;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintTransform _transform;
        private readonly IPaintImage _image;
        private readonly IPaintMerging _merging;
        private readonly PaintDropOptions _options;

        public IStateDefinition Definition { get; }

        public async UniTask Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            var center = target.GetNearestCenter(_transform.Value);

            var start = _transform.WorldPosition;
            var end = center.Transform.position;

            _merging.Show(handle.Lifetime, target);
            
            await _updater.CurveProgression(handle.Lifetime, _options.DockScaleCurve,
                progress => _transform.SetWorldPosition(Vector2.Lerp(start, end, progress)));
            
            handle.Exit();
        }
    }
}