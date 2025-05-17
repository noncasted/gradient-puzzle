using Global.UI;
using Internal;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Levels
{
    [DisallowMultipleComponent]
    public class LevelSelectionView : MonoBehaviour 
    {
        [SerializeField] private Image _preview;
        [SerializeField] private RectTransform _transform;
        [SerializeField] private DesignButton _button;

        private ILevelData _level;

        public IViewableDelegate Clicked => _button.Clicked;
        public ILevelData Level => _level;

        public void Setup(int index, float height, ILevelData level)
        {
            _level = level;
            _transform.anchoredPosition = new Vector2(0, height);
        }
    }
}