using System.Linq;
using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using Menu.Levels;
using Services;
using UnityEngine;
using VContainer;

namespace Menu.Sections
{
    [DisallowMultipleComponent]
    public class LevelSections : MonoBehaviour, ILevelSections, ISceneService
    {
        [SerializeField] private LevelSectionView _sectionPrefab;
        [SerializeField] private Transform _root;
        [SerializeField] private DesignButton _back;

        private IScriptableRegistry<LevelSectionOptions> _sections;
        private IUIStateMachine _stateMachine;
        private ILevelSelection _levelSelection;
        private ILevelsStorage _levels;

        public IUIConstraints Constraints => UIConstraints.Game;

        [Inject]
        private void Construct(
            IUIStateMachine stateMachine,
            ILevelSelection levelSelection,
            ILevelsStorage levels,
            IScriptableRegistry<LevelSectionOptions> sections)
        {
            _levels = levels;
            _levelSelection = levelSelection;
            _stateMachine = stateMachine;
            _sections = sections;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ILevelSections>()
                .WithScriptableRegistry<LevelSectionRegistry, LevelSectionOptions>();
        }

        public UniTask<ILevelData> Show(IUIStateHandle handle, bool withBackOptions)
        {
            handle.AttachGameObject(gameObject);
            _back.gameObject.SetActive(withBackOptions);

            var completion = new UniTaskCompletionSource<ILevelData>();

            _back.Clicked.Advise(handle.InnerLifetime, () => completion.TrySetResult(null));

            var progress = _levels.CalculateProgress();

            var sections = _sections.Objects.OrderBy(t => (int)t.Type);
            
            foreach (var options in sections)
            {
                var view = Instantiate(_sectionPrefab, _root);
                view.Setup(options, progress[options.Type]);

                view.Clicked.Advise(handle.InnerLifetime, () => OnClicked(options).Forget());
            }

            async UniTask OnClicked(LevelSectionOptions options)
            {
                var selection = await _stateMachine.ProcessStack(
                    this,
                    _levelSelection,
                    selectionHandle => _levelSelection.Show(selectionHandle, options.Type));

                if (selection == null)
                {
                    handle.Exit();
                    return;
                }

                completion.TrySetResult(selection);
            }

            return completion.Task;
        }

        public UniTask OnEntered(IUIStateHandle handle)
        {
            throw new System.NotImplementedException();
        }
    }
}