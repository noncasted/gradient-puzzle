using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using UnityEngine;
using VContainer;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class CompletionUI : MonoBehaviour, ICompletionUI, IUIStateAsyncEnterHandler, ISceneService
    {
        [SerializeField] private DesignButton _nextButton;
        
        private IOverlayBackground _background;

        public IUIConstraints Constraints { get; } = UIConstraints.Game;
        
        [Inject]
        private void Construct(IOverlayBackground background)
        {
            _background = background;
        }
        
        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ICompletionUI>();
            
            gameObject.SetActive(false);
        }
        
        public UniTask OnEntered(IUIStateHandle handle)
        {
            handle.AttachGameObject(gameObject);
            _background.Show(handle);

            return _nextButton.WaitClick(handle);
        }
    }
}