using Global.UI;
using Internal;
using MPUIKIT;
using UnityEngine;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class OverlayBackground : MonoBehaviour, IOverlayBackground, ISceneService
    {
        [SerializeField] private float _targetAlpha;
        [SerializeField] private float _duration;
        [SerializeField] private MPImage _image;

        private IUIState _source;
        private bool _show;
        private float _time;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IOverlayBackground>();
        }

        public void Show(IUIState source)
        {
            _source = source;
            _show = true;
            gameObject.SetActive(true);
        }

        public void Hide(IUIState source)
        {
            if (_source != source)
                return;

            _show = false;
        }

        private void Update()
        {
            if (_show == true)
                _time += Time.deltaTime;
            else
                _time -= Time.deltaTime;

            _time = Mathf.Clamp(_time, 0f, _duration);

            var progress = Mathf.Clamp01(_time / _duration);
            var color = _image.color;
            color.a = Mathf.Lerp(0f, _targetAlpha / 255f, progress);
            _image.color = color;

            if (_time <= 0f)
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }
}