using Features.Services;
using Global.UI;
using Internal;
using TMPro;
using UnityEngine;

namespace Features
{
    [DisallowMultipleComponent]
    public class LevelSelectionEntry : MonoBehaviour
    {
        [SerializeField] private GameObject _lockPlate;
        [SerializeField] private DesignButton _button;
        [SerializeField] private TMP_Text _text;

        public IViewableDelegate Clicked => _button.Clicked;

        public void Construct(IReadOnlyLifetime lifetime, ILevelConfiguration configuration)
        {
            _text.text = configuration.Index.ToString();

            configuration.IsUnlocked.View(lifetime, isUnlocked =>
            {
                if (isUnlocked == true)
                    _button.Unlock();
                else 
                    _button.Lock();
                
                _lockPlate.SetActive(isUnlocked == false);
            });
        }
    }
}