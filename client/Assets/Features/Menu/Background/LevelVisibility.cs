using Internal;
using UnityEngine;

namespace Menu
{
    public interface ILevelVisibility
    {
        void Show();
        void Hide();
    }

    [DisallowMultipleComponent]
    public class LevelVisibility : MonoBehaviour, ISceneService, ILevelVisibility
    {
        [SerializeField] private float _switchTime = 0.5f;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _isVisible;
        private float _timer;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<ILevelVisibility>();
        }

        public void Show()
        {
            _isVisible = true;
        }

        public void Hide()
        {
            _isVisible = false;
        }

        private void Update()
        {
            _timer += _isVisible == true ? Time.deltaTime : -Time.deltaTime;
            _timer = Mathf.Clamp(_timer, 0f, _switchTime);
            var progress = _timer / _switchTime;
            _canvasGroup.alpha = progress;
        }
    }
}