using Internal;
using UnityEngine;

namespace Menu
{
    public interface IMenuNavigation
    {
        void Show();
        void Hide();
    }
    
    [DisallowMultipleComponent]
    public class MenuNavigation : MonoBehaviour, IMenuNavigation, ISceneService
    {
        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IMenuNavigation>();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}