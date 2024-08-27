using Cysharp.Threading.Tasks;
using Features.Completion;
using Features.Services;
using Global.UI;
using Internal;
using UnityEngine;
using VContainer;

namespace Features
{
    [DisallowMultipleComponent]
    public class MainOverlay : MonoBehaviour, IMainOverlay, IUIStateEnterHandler, ISceneService
    {
        [SerializeField] private DesignButton _settings;
        [SerializeField] private DesignButton _levels;
        [SerializeField] private DesignButton _reset;

        private readonly ViewableDelegate<ILevelConfiguration> _levelSelected = new();

        private IUIStateMachine _stateMachine;
        private ISettingsUI _settingsUI;
        private ILevelSelectionUI _levelSelectionUI;
        private ICompletionUI _completion;

        public IUIConstraints Constraints => new UIConstraints();

        public IViewableDelegate ResetClicked => _reset.Clicked;
        public IViewableDelegate<ILevelConfiguration> LevelSelected => _levelSelected;

        [Inject]
        private void Construct(
            IUIStateMachine stateMachine,
            ISettingsUI settings,
            ILevelSelectionUI levelSelection,
            ICompletionUI completion)
        {
            _completion = completion;
            _stateMachine = stateMachine;
            _levelSelectionUI = levelSelection;
            _settingsUI = settings;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IMainOverlay>();
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
            _levels.ListenClick(handle, () => HandleSelection().Forget());

            return;

            async UniTask HandleSelection()
            {
                var result = await _stateMachine.ProcessChild(
                    this,
                    _levelSelectionUI,
                    stateHandle => _levelSelectionUI.Process(stateHandle));

                if (result.IsSelected == false)
                    return;

                _levelSelected.Invoke(result.Level);
            }
        }
    }
}