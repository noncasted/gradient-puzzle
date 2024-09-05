using Cysharp.Threading.Tasks;
using Features.Services;
using Global.UI;
using Internal;
using UnityEngine;
using VContainer;

namespace Features
{
    [DisallowMultipleComponent]
    public class LevelSelectionUI : MonoBehaviour, ILevelSelectionUI, IScopeSetupCompletion, ISceneService
    {
        [SerializeField] private LevelSelectionEntry _prefab;
        [SerializeField] private LevelSelectionScaler _scaler;
        [SerializeField] private DesignButton _closeButton;

        private IOverlayBackground _background;
        private UniTaskCompletionSource<int> _selected;
        private ILevelsStorage _levelsStorage;

        public IUIConstraints Constraints { get; } = UIConstraints.Game;

        [Inject]
        private void Construct(IOverlayBackground background, ILevelsStorage levelsStorage)
        {
            _levelsStorage = levelsStorage;
            _background = background;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ILevelSelectionUI>()
                .As<IScopeSetupCompletion>();

            gameObject.SetActive(false);
        }
        
        public void OnSetupCompletion(IReadOnlyLifetime lifetime)
        {
            for (var i = 0; i < _levelsStorage.Count(); i++)
            {
                var entry = Instantiate(_prefab, _scaler.transform);
                entry.Construct(lifetime, _levelsStorage.Configurations[i]);
                var index = i;

                entry.Clicked.Advise(lifetime, () => { _selected.TrySetResult(index); });
            }

            _scaler.Rescale(_levelsStorage.Count());
        }

        public UniTask<LevelSelectionResult> Process(IUIStateHandle handle)
        {
            _background.Show(handle);
            handle.AttachGameObject(gameObject);

            var completion = new UniTaskCompletionSource<LevelSelectionResult>();
            handle.InnerLifetime.Listen(() => completion.TrySetResult(new LevelSelectionResult()));

            HandleSelection().Forget();
            HandleCancel().Forget();

            return completion.Task;

            async UniTask HandleSelection()
            {
                _selected = new UniTaskCompletionSource<int>();
                handle.InnerLifetime.Listen(() => _selected.TrySetResult(-1));
                var level = await _selected.Task;

                if (level == -1)
                    return;

                completion.TrySetResult(new LevelSelectionResult(_levelsStorage.Get(level)));
            }

            async UniTask HandleCancel()
            {
                await _closeButton.WaitClick(handle.InnerLifetime);
                completion.TrySetResult(new LevelSelectionResult());
            }
        }
    }
}