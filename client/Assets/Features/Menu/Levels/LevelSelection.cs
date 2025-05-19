using System.Linq;
using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using Services;
using UnityEngine;
using VContainer;

namespace Menu.Levels
{
    [DisallowMultipleComponent]
    public class LevelSelection : MonoBehaviour, ISceneService, ILevelSelection
    {
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
            handle.AttachGameObject(gameObject);
            
            var previous = GetComponentsInChildren<LevelSelectionView>();
            
            foreach (var view in previous)
                Destroy(view.gameObject);
            
            var completion = new UniTaskCompletionSource<ILevelData>();
            var levels = _levels.Sections[sectionType];
        
            _back.Clicked.Advise(handle.InnerLifetime, () => completion.TrySetResult(null));

            for (var index = levels.Count - 1; index >= 0; index--)
            {
                var level = levels[index];
                var view = Instantiate(_entryPrefab, _root);
                view.Setup(index, level, index != 0);

                view.Clicked.Advise(handle.InnerLifetime, () => completion.TrySetResult(level));
            }

            return completion.Task;
        }
    }
}