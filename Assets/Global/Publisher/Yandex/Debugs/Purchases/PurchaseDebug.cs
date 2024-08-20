using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Global.Publisher
{
    [DisallowMultipleComponent]
    public class PurchaseDebug : MonoBehaviour, IPurchaseDebug
    {
        [SerializeField] private GameObject _body;

        [SerializeField] private TMP_Text _key;

        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _closeButton;

        private YandexCallbacks _callbacks;

        private string _current;

        private void OnEnable()
        {
            _acceptButton.onClick.AddListener(OnAcceptClicked);
            _closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnDisable()
        {
            _acceptButton.onClick.RemoveListener(OnAcceptClicked);
            _closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        public void Construct(YandexCallbacks callbacks)
        {
            _callbacks = callbacks;
        }

        public void Purchase(string key)
        {
            _body.SetActive(true);

            _key.text = $"key: {key}";

            _current = key;
        }

        private void OnAcceptClicked()
        {
            Close();

            _callbacks.OnPurchaseSuccess("Closed");
        }

        private void OnCloseClicked()
        {
            _callbacks.OnPurchaseFailed(_current);
        }

        private void Close()
        {
            _body.SetActive(false);
        }
    }
}