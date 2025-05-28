using Global.UI;
using Internal;
using MPUIKIT;
using NaughtyAttributes;
using UnityEngine;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class CompletionStar : MonoBehaviour
    {
        [SerializeField] [CurveRange] private AnimationCurve _curve;
        
        [SerializeField] private float _switchTime = 0.3f;
        [SerializeField] private int _index;
        [SerializeField] private DesignButton _button;
        [SerializeField] private MPImage _image;

        private float _timer;
        private bool _isActive;
        
        private readonly ViewableDelegate<int> _click = new();
        
        public IViewableDelegate<int> Click => _click;

        private void OnEnable()
        {
            var lifetime = this.GetObjectLifetime();
            _button.ListenClick(lifetime, () => _click.Invoke(_index));
        }

        public void Show()
        {
            _isActive = true;
        }

        public void Hide()
        {
            _isActive = false;
        }

        public void ForceHide()
        {
            _timer = 0f;
            _isActive = false;
            
            SetAlpha(0);
        }

        private void Update()
        {
            if (_isActive == true && Mathf.Approximately(_timer, _switchTime) || 
                _isActive == false && Mathf.Approximately(_timer, 0f))
                return;
            
            _timer += _isActive == true ? Time.deltaTime : -Time.deltaTime;
            _timer = Mathf.Clamp(_timer, 0f, _switchTime);
            var progress = _timer / _switchTime;
            
            var factor = _curve.Evaluate(progress);
            var alpha = Mathf.Lerp(0f, 1f, factor);

            SetAlpha(alpha);
        }

        private void SetAlpha(float value)
        {
            var firstKey = new GradientAlphaKey()
            {
                alpha = value,
                time = 0f
            };
            
            var secondKey = new GradientAlphaKey()
            {
                alpha = value,
                time = 1f
            };
            
            _image.GradientEffect.Gradient.alphaKeys = new[] { firstKey, secondKey };
            _image.SetAllDirty();
        }
    }
}