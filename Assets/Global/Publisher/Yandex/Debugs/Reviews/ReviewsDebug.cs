using UnityEngine;
using UnityEngine.UI;

namespace Global.Publisher.Yandex
{
    [DisallowMultipleComponent]
    public class ReviewsDebug : MonoBehaviour, IReviewsDebug
    {
        [SerializeField] private GameObject _body;

        [SerializeField] private Button _sendButton;

        private YandexCallbacks _callbacks;

        private void OnEnable()
        {
            _sendButton.onClick.AddListener(OnSendClicked);
        }

        private void OnDisable()
        {
            _sendButton.onClick.RemoveListener(OnSendClicked);
        }

        public void Construct(YandexCallbacks callbacks)
        {
            _callbacks = callbacks;
        }

        public void Review()
        {
            _body.SetActive(true);
        }

        private void OnSendClicked()
        {
            _body.SetActive(false);
            _callbacks.OnReview();
        }
    }
}