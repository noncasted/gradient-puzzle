using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Internal
{
    [DisallowMultipleComponent]
    public class ServiceComponentsRoot : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _autoDetected;
        
        public void Create(IScopeBuilder builder)
        {
            foreach (var behaviour in _autoDetected)
            {
                if (behaviour is not IServiceComponent component)
                    throw new Exception();

                component.Register(builder);
            }
        }
        
        [Button("Scan")]
        private void OnValidate()
        {
            _autoDetected.Clear();
            var components = GetComponentsInChildren<IServiceComponent>(true);

            foreach (var component in components)
            {
                if (component is not MonoBehaviour behaviour)
                    throw new Exception();

                _autoDetected.Add(behaviour);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}