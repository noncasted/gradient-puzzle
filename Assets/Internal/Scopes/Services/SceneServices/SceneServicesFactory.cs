using UnityEngine;

namespace Internal
{
    public class SceneServicesFactory : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _services;

        public void Create(IScopeBuilder builder)
        {
            foreach (var service in _services)
            {
                if (service is not ISceneService sceneService)
                    continue;
                
                sceneService.Create(builder);
            }
        }

        public void SetServices(MonoBehaviour[] services)
        {
            _services = services;
        }
    }
}