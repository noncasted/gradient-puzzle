using System.Runtime.InteropServices;

namespace Global.Publisher
{
    public class LeaderboardsExternAPI : ILeaderboardsAPI
    {
        [DllImport("__Internal")]
        private static extern void SetLeaderboard(string target, int score);

        [DllImport("__Internal")]
        private static extern void GetLeaderboardEntries(string target, int quantityTop, int quantityAround);

        public void SetLeaderboard_Internal(string target, int score)
        {
            SetLeaderboard(target, score);
        }

        public void GetLeaderboard_Internal(string target, int quantityTop, int quantityAround)
        {
            GetLeaderboardEntries(target, quantityTop, quantityAround);
        }
    }
}