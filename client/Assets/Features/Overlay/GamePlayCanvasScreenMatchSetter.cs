using Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Overlay
{ 
    [DisallowMultipleComponent]
    public class GamePlayCanvasScreenMatchSetter : MonoBehaviour, ISceneService
    {
        [SerializeField] private CanvasScaler[] _scalers;
        
        public void Create(IScopeBuilder builder)
        {
            var resolution = Screen.currentResolution.height / (float)Screen.currentResolution.width;
            
            if (resolution > 1.3f)
            {
                foreach (var canvasScaler in _scalers)
                    canvasScaler.matchWidthOrHeight = 0f;
            }
            else
            {
                foreach (var canvasScaler in _scalers)
                    canvasScaler.matchWidthOrHeight = 1f;
            }
        }
    }
}