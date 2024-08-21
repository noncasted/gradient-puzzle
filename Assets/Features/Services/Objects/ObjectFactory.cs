using Internal;
using UnityEngine;

namespace Features.Services
{
    public abstract class ObjectFactory<T> : MonoBehaviour, ISceneService, IObjectFactory<T> where T : MonoBehaviour
    {
        private int _counter;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IObjectFactory<T>>();
        }

        public T Create(T prefab, Vector2 position, float angle)
        {
            var instance = Instantiate(prefab, position, Quaternion.Euler(0, 0, angle), transform);
            _counter++;
            instance.name = $"{prefab.name}_{_counter}";
            return instance;
        }
    }
}