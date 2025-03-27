using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Selections
{
    public class Testasdasd :MonoBehaviour
    {
        [SerializeField] private Vector2 _position;

        [Button]
        private void Get()
        {
            _position = transform.position;
        }
    }
}