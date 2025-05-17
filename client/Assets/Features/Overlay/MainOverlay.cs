using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using Menu.Sections;
using Services;
using UnityEngine;
using VContainer;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class MainOverlay : MonoBehaviour, IMainOverlay, IUIStateEnterHandler, ISceneService
    {
        [SerializeField] private DesignButton _settings;
        [SerializeField] private DesignButton _levels;
        [SerializeField] private DesignButton _reset;

        private readonly ViewableDelegate<ILevelData> _levelSelected = new();

        private IUIStateMachine _stateMachine;
        private ISettingsUI _settingsUI;
        private ICompletionUI _completion;
        private ILevelSections _levelSections;
        private IGameContext _gameContext;

        public IUIConstraints Constraints => new UIConstraints();

        public IViewableDelegate ResetClicked => _reset.Clicked;
        public IViewableDelegate<ILevelData> LevelSelected => _levelSelected;

        [Inject]
        private void Construct(
            IUIStateMachine stateMachine,
            IGameContext gameContext,
            ISettingsUI settings,
            ILevelSections levelSections,
            ICompletionUI completion)
        {
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
            var result = await _stateMachine.ProcessStack(
                this,
                _levelSections,
                stateHandle => _levelSections.Show(stateHandle, _gameContext.Level != null));

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
            _levels.ListenClick(handle, () => ShowSections().Forget());
        }
    }
}