using Global.UI;
using Internal;
using Services;
using TMPro;
using UnityEngine;

namespace Overlay
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
            
            lifetime.Listen(() => Debug.Log("Terminated"));

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