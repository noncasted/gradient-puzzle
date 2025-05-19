using Global.UI;
using Internal;
using MPUIKIT;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Levels
{
    [DisallowMultipleComponent]
    public class LevelSelectionView : MonoBehaviour 
    {
        [SerializeField] private Color _passedColor;
        [SerializeField] private Image _preview;
        [SerializeField] private RectTransform _previewTransform;
        [SerializeField] private DesignButton _button;
        [SerializeField] private MPImage _buttonImage;
        [SerializeField] private MPImage _linkLine;
        [SerializeField] private TMP_Text _indexText;

        private ILevelData _level;

        public IViewableDelegate Clicked => _button.Clicked;
        public ILevelData Level => _level;

        public void Setup(int index, ILevelData level, bool showLine)
        {
            _level = level;
            _indexText.text = (index + 1).ToString();
            _preview.sprite = level.Options.Preview;

            if (level.IsPassed.Value == true)
            {
                _buttonImage.color = _passedColor;
                _indexText.color = _passedColor;
                
                // TODO: Enable link color when level progression will work
                //_linkLine.color = _passedColor;
            }
            
            if (index % 2 == 1)
            {
                var position = _previewTransform.anchoredPosition;
                _previewTransform.anchoredPosition = new Vector2(-position.x, position.y);
            }
            
            _linkLine.gameObject.SetActive(showLine);
        }
    }
}