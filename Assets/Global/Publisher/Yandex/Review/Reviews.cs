using Cysharp.Threading.Tasks;

namespace Global.Publisher
{
    public class Reviews : IReviews
    {
        public Reviews(YandexCallbacks callbacks, IReviewsAPI api)
        {
            _callbacks = callbacks;
            _api = api;
        }

        private readonly YandexCallbacks _callbacks;
        private readonly IReviewsAPI _api;

        private bool _isReviewed;

        public async UniTask Review()
        {
            // if (_isReviewed == true)
            //     return;
            //
            // _pause.Pause();
            //
            // var completion = new UniTaskCompletionSource();
            //
            // void OnReviewed()
            // {
            //     completion.TrySetResult();
            // }
            //
            // _callbacks.Reviewed += OnReviewed;
            //
            // _api.Review_Internal();
            //
            // _isReviewed = true;
            //
            // await completion.Task;
            //
            // _callbacks.Reviewed -= OnReviewed;
            //
            // _pause.Continue();
        }
    }
}