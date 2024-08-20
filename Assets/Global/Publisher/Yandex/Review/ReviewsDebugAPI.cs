namespace Global.Publisher
{
    public class ReviewsDebugAPI : IReviewsAPI
    {
        public ReviewsDebugAPI(IReviewsDebug debug)
        {
            _debug = debug;
        }

        private readonly IReviewsDebug _debug;

        public void Review_Internal()
        {
            _debug.Review();
        }
    }
}