using System.Runtime.InteropServices;

namespace Global.Publisher.Yandex
{
    public class ReviewsExternAPI : IReviewsAPI
    {
        [DllImport("__Internal")]
        private static extern void Review();

        public void Review_Internal()
        {
            Review();
        }
    }
}