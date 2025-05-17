using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using Services;
using UnityEngine;
using VContainer;

namespace Menu.Levels
{
    public class LevelSelection : MonoBehaviour, ISceneService, ILevelSelection
    {
        [SerializeField] private float _distanceBetweenEntries;
        [SerializeField] private LevelSelectionView _entryPrefab;
        [SerializeField] private Transform _root;
        [SerializeField] private DesignButton _back;
        
        private ILevelsStorage _levels;
        
        public IUIConstraints Constraints => UIConstraints.Game;

        [Inject]
        private void Construct(ILevelsStorage levels)
        {
            _levels = levels;
        }
        
        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ILevelSelection>();
        }
        
        public UniTask<ILevelData> Show(IUIStateHandle handle, LevelSectionType sectionType)
        {
            var previous = GetComponentsInChildren<LevelSelectionView>();
            
            foreach (var view in previous)
                Destroy(view.gameObject);
            
            var completion = new UniTaskCompletionSource<ILevelData>();
            var levels = _levels.Sections[sectionType];
        
            _back.Clicked.Advise(handle.InnerLifetime, () => completion.TrySetResult(null));

            for (var index = 0; index < levels.Count; index++)
            {
                var level = levels[index];
                var height = index * _distanceBetweenEntries;
                var view = Instantiate(_entryPrefab, _root);
                view.Setup(index, height, level);

                view.Clicked.Advise(handle.InnerLifetime, () => completion.TrySetResult(level));
            }

            return completion.Task;
        }
    }
}