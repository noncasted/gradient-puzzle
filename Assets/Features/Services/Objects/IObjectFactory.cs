using UnityEngine;

namespace Features.Services
{
    public interface IObjectFactory<T> where T : MonoBehaviour
    {
        T Create(T prefab, Vector2 position, float angle);
        void DestroyAll();
    }

    public static class ObjectFactoryExtensions
    {
        public static T Create<T>(this IObjectFactory<T> factory, T prefab, Vector2 position) where T : MonoBehaviour
        {
            return factory.Create(prefab, position, 0f);
        }

        public static T Create<T>(this IObjectFactory<T> factory, T prefab) where T : MonoBehaviour
        {
            var result = factory.Create(prefab, Vector3.zero, 0f);
            result.transform.localPosition = Vector3.zero;
            return result;  
        }
    }
}