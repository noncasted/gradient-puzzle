using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;
using Global.Systems;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintComplete : MonoBehaviour, IPaintComplete, IState
    {
        public PaintComplete(
            IStateMachine stateMachine,
            IPaintImage image,
            IUpdater updater,
            PaintCompleteOptions options,
            PaintCompleteDefinition definition)
        {
            Definition = definition;
            _stateMachine = stateMachine;
            _image = image;
            _updater = updater;
            _options = options;
        }

        private readonly IStateMachine _stateMachine;
        private readonly IPaintImage _image;
        private readonly IUpdater _updater;
        private readonly PaintCompleteOptions _options;

        public IStateDefinition Definition { get; }

        public UniTask Process()
        {
            var stateHandle = _stateMachine.CreateHandle(this);

            var startColor = _image.Color;
            var colorAdjustment = _options.CompletionColorAdjustment / 255f;

            return _updater.CurveProgression(stateHandle.Lifetime, _options.CompletionCurve, progress =>
            {
                var color = startColor;
                var add = Mathf.Lerp(0, colorAdjustment, progress);
                color.r += add;
                color.g += add;
                color.b += add;

                _image.SetColor(color);
            });
        }
    }
}