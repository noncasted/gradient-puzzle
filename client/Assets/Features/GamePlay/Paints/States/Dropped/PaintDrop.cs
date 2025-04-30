using Common.StateMachines;
using Cysharp.Threading.Tasks;
using GamePlay.Common;
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
            var center = target.GetNearestCenter(_transform.RectPosition);

            var start = _transform.RectPosition;
            var end = center;
            
            _image.Hide();

            _merging.Show(new PaintMergingHandleOptions()
            {
                Lifetime = handle.Lifetime,
                ShowBody = true,
                Targets = new[] { target },
                WithInit = false
            });
            
            await _updater.CurveProgression(handle.Lifetime, _options.DockScaleCurve,
                progress => _transform.SetRectPosition(Vector2.Lerp(start, end, progress)));
            
            handle.Exit();
        }
    }
}