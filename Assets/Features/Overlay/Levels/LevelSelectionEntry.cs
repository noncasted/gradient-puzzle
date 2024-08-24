using Global.UI;
using Internal;
using TMPro;
using UnityEngine;

namespace Features
{
    [DisallowMultipleComponent]
    public class LevelSelectionEntry : MonoBehaviour
    {
        [SerializeField] private DesignButton _button;
        [SerializeField] private TMP_Text _text;

        public IViewableDelegate Clicked => _button.Clicked;

        public void Construct(int level)
        {
            _text.text = level.ToString();
        }
    }
}