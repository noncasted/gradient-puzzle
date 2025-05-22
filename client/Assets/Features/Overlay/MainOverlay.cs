using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using Menu;
using Menu.Sections;
using Services;
using UnityEngine;
using VContainer;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class MainOverlay : MonoBehaviour, IMainOverlay, IUIStateEnterHandler, ISceneService
    {
        [SerializeField] private GameObject _navigation;
        [SerializeField] private DesignButton _settings;
        [SerializeField] private DesignButton _levels;
        [SerializeField] private DesignButton _reset;

        private readonly ViewableDelegate<ILevelData> _levelSelected = new();

        private IUIStateMachine _stateMachine;
        private ISettingsUI _settingsUI;
        private ICompletionUI _completion;
        private ILevelSections _levelSections;
        private IGameContext _gameContext;
        private IBackground _background;
        private ILevelVisibility _levelVisibility;

        public IUIConstraints Constraints => new UIConstraints();

        public IViewableDelegate ResetClicked => _reset.Clicked;
        public IViewableDelegate<ILevelData> LevelSelected => _levelSelected;

        [Inject]
        private void Construct(
            IUIStateMachine stateMachine,
            IGameContext gameContext,
            ISettingsUI settings,
            ILevelSections levelSections,
            IBackground background,
            ILevelVisibility levelVisibility,
            ICompletionUI completion)
        {
            _levelVisibility = levelVisibility;
            _background = background;
            _gameContext = gameContext;
            _levelSections = levelSections;
            _completion = completion;
            _stateMachine = stateMachine;
            _settingsUI = settings;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IMainOverlay>();
        }

        public async UniTask ShowSections()
        {
            _background.ToSections();
            _navigation.SetActive(false);
            _levelVisibility.Hide();

            var result = await _stateMachine.ProcessStack(
                this,
                _levelSections,
                stateHandle => _levelSections.Show(stateHandle, _gameContext.Level != null));

            _navigation.SetActive(true);
            _levelVisibility.Show();

            if (result == null)
                return;

            _levelSelected.Invoke(result);
        }

        public void ShowReset()
        {
            _reset.gameObject.SetActive(true);
        }

        public void HideReset()
        {
            _reset.gameObject.SetActive(false);
        }

        public void OnEntered(IUIStateHandle handle)
        {
            _settings.ListenClick(handle, () => _stateMachine.Process(this, _settingsUI));
            _levels.ListenClick(handle, async () => { await ShowSections(); });
        }
    }
}