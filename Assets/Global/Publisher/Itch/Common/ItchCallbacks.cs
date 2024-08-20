using System;
using UnityEngine;

namespace Global.Publisher
{
    [DisallowMultipleComponent]
    public class ItchCallbacks : MonoBehaviour, IJsErrorCallback
    {
        public event Action<string> Exception; 
        
        public void OnException(string exception)
        {
            Exception?.Invoke(exception);
        }
    }
}