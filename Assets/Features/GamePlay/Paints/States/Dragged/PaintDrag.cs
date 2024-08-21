using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;
using Features.Services.Inputs;
using Global.Systems;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintDrag : IPaintDrag, IState
    {
        public PaintDrag(
            IUpdater updater,
            IGameInput input,
            IStateMachine stateMachine,
            IPaintImage image,
            IPaintTransform transform,
            IPaintMoveArea moveArea,
            PaintDragOptions options,
            PaintDragDefinition definition)
        {
            Definition = definition;
            _updater = updater;
            _input = input;
            _stateMachine = stateMachine;
            _image = image;
            _transform = transform;
            _moveArea = moveArea;
            _options = options;
        }

        private readonly IUpdater _updater;
        private readonly IGameInput _input;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintImage _image;
        private readonly IPaintTransform _transform;
        private readonly IPaintMoveArea _moveArea;
        private readonly PaintDragOptions _options;

        public IStateDefinition Definition { get; }

        public void Enter(IPaintMoveHandle handle)
        {
            var stateHandle = _stateMachine.CreateHandle(this);
            var lifetime = stateHandle.Lifetime.Intersect(handle.Lifetime);
            Process(lifetime, handle).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime, IPaintMoveHandle handle)
        {
            _image.ToCircle();

            if (handle.Source is IPaintDock)
            {
                _transform.AttachTo(_moveArea.Transform);
                await ScaleDown();
            }
            else
            {
                await ScaleDown();
                _transform.AttachTo(_moveArea.Transform);
            }
            
            await _updater.RunUpdateAction(lifetime, Move);

            async UniTask ScaleDown()
            {
                var curve = _options.DragSizeCurve.CreateInstance();
                var startSize = _image.Size;

                await _updater.RunUpdateAction(lifetime, () => !curve.IsFinished, delta =>
                {
                    var factor = curve.Step(delta);
                    var size = Mathf.Lerp(startSize, _options.DragSize, factor);
                    _image.SetSize(size);

                    Move(delta);
                });
            }

            void Move(float delta)
            {
                var targetPosition = _input.CursorPosition;
                var currentPosition = _transform.Position;
                
                var move = _options.MoveSpeed * delta;
                var position = Vector2.Lerp(currentPosition, targetPosition, move);

                _transform.SetPosition(position);
            }
        }
    }
}