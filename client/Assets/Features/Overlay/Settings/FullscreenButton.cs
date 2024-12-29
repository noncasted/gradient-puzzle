using System.Runtime.InteropServices;
using Global.UI;
using Internal;
using MarksAssets.FullscreenWebGL;
using UnityEngine;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class FullscreenButton : MonoBehaviour
    {
        [SerializeField] private DesignButton _button;
        
        [DllImport("__Internal")]
        private static extern void RequestFullscreen();

        private void OnEnable()
        {
            var lifetime = this.GetObjectLifetime();
            _button.ListenClick(lifetime, () => FullscreenWebGL.requestFullscreen(_ => { }, FullscreenWebGL.navigationUI.hide));
        }
    }
}