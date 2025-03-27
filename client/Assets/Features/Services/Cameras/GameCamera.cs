using Internal;
using UnityEngine;

namespace Services
{
    [DisallowMultipleComponent]
    public class GameCamera : MonoBehaviour, IGameCamera, ISceneService
    {
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;
        
        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IGameCamera>();
        }
    }
}