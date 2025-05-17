using Global.UI;
using Internal;
using MPUIKIT;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Sections
{
    [DisallowMultipleComponent]
    public class LevelSectionView : MonoBehaviour
    {
        [SerializeField] private Image _preview;
        [SerializeField] private MPImage _plate;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private DesignButton _button;

        public IViewableDelegate Clicked => _button.Clicked;

        public void Setup(LevelSectionOptions options, int progress)
        {
            _preview.sprite = options.Preview;
            _title.text = options.Type.ToName();
            _progress.text = $"{progress} %";
            
            var effect = _plate.GradientEffect;
            effect.Gradient = options.PreviewGradient;
            _plate.GradientEffect = effect;
        }
    }
}