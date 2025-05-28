using System;
using Cysharp.Threading.Tasks;
using Global.Metrics;
using Global.UI;
using Internal;
using Services;
using Shared;
using UnityEngine;
using VContainer;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class CompletionUI : MonoBehaviour, ICompletionUI, IUIStateAsyncEnterHandler, ISceneService
    {
        [SerializeField] private DesignButton _nextButton;
        [SerializeField] private CompletionStar[] _stars;

        private IOverlayBackground _background;
        private IMetrics _metrics;
        private IGameContext _gameContext;

        public IUIConstraints Constraints { get; } = UIConstraints.Game;

        [Inject]
        private void Construct(
            IMetrics metrics,
            IGameContext gameContext,
            IOverlayBackground background)
        {
            _gameContext = gameContext;
            _metrics = metrics;
            _background = background;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ICompletionUI>();

            gameObject.SetActive(false);
        }

        public async UniTask OnEntered(IUIStateHandle handle)
        {
            handle.AttachGameObject(gameObject);

            _background.Show(handle);

            var rating = -1;

            foreach (var star in _stars)
            {
                star.ForceHide();
                star.Click.Advise(handle.InnerLifetime, OnStarClicked);
            }

            await _nextButton.WaitClick(handle);

            if (rating != -1)
            {
                _metrics.Send(new MetricsContexts.LevelRate()
                {
                    Section = _gameContext.LevelData.Options.SectionType, 
                    LevelIndex = _gameContext.LevelData.Index,
                    Rate = IndexToRating(rating)
                }).Forget();
            }

            void OnStarClicked(int index)
            {
                rating = index;
                
                for (var i = 0; i < _stars.Length; i++)
                {
                    var star = _stars[i];

                    if (i <= index)
                        star.Show();
                    else
                        star.Hide();
                }
            }
            
            LevelRating IndexToRating(int index)
            {
                return index switch
                {
                    0 => LevelRating.Bad,
                    1 => LevelRating.Poor,
                    2 => LevelRating.Good,
                    3 => LevelRating.Good,
                    4 => LevelRating.Good,
                    _ => throw new Exception()
                };
            }
        }
    }
}