using System.Collections.Generic;
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
    public class LevelSections : MonoBehaviour, ILevelSections, ISceneService, IScopeSetup
    {
        [SerializeField] private LevelSectionView _sectionPrefab;
        [SerializeField] private Transform _root;
        [SerializeField] private DesignButton _back;

        private readonly Dictionary<LevelSectionType, LevelSectionView> _sectionsViews = new();

        private IScriptableRegistry<LevelSectionOptions> _sections;
        private IUIStateMachine _stateMachine;
        private ILevelSelection _levelSelection;
        private ILevelsStorage _levels;
        private IBackground _background;

        public IUIConstraints Constraints => UIConstraints.Game;

        [Inject]
        private void Construct(
            IUIStateMachine stateMachine,
            ILevelSelection levelSelection,
            ILevelsStorage levels,
            IBackground background,
            IScriptableRegistry<LevelSectionOptions> sections)
        {
            _background = background;
            _levels = levels;
            _levelSelection = levelSelection;
            _stateMachine = stateMachine;
            _sections = sections;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ILevelSections>()
                .As<IScopeSetup>()
                .WithScriptableRegistry<LevelSectionRegistry, LevelSectionOptions>();
        }

        public void OnSetup(IReadOnlyLifetime lifetime)
        {
            var sections = _sections.Objects.OrderBy(t => (int)t.Type);

            foreach (var options in sections)
            {
                var view = Instantiate(_sectionPrefab, _root);
                view.Setup(options);

                _sectionsViews.Add(options.Type, view);
            }
        }

        public UniTask<ILevelData> Show(IUIStateHandle handle, bool withBackOptions)
        {
            handle.AttachGameObject(gameObject);
            _back.gameObject.SetActive(withBackOptions);

            var completion = new UniTaskCompletionSource<ILevelData>();

            _back.Clicked.Advise(handle.InnerLifetime, () => completion.TrySetResult(null));

            var progress = _levels.CalculateProgress();

            foreach (var (type, view) in _sectionsViews)
            {
                view.UpdateProgress(progress[type]);
                view.Clicked.Advise(handle.InnerLifetime, () => OnClicked(type).Forget());
            }

            async UniTask OnClicked(LevelSectionType sectionType)
            {
                _background.ToLevels();
                var selection = await _stateMachine.ProcessStack(
                    this,
                    _levelSelection,
                    selectionHandle => _levelSelection.Show(selectionHandle, sectionType));

                if (selection == null)
                {
                    _background.ToSections();
                    return;
                }

                completion.TrySetResult(selection);
            }

            return completion.Task;
        }
    }
}