using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using UnityEngine;
using VContainer;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class SettingsUI : MonoBehaviour, ISettingsUI, IUIStateAsyncEnterHandler, ISceneService
    {
        [SerializeField] private DesignButton _closeButton;
        
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
                .As<ISettingsUI>();
            
            gameObject.SetActive(false);
        }
        
        public UniTask OnEntered(IUIStateHandle handle)
        {
            handle.AttachGameObject(gameObject);
            _background.Show(handle);

            return _closeButton.WaitClick(handle);
        }
    }
}