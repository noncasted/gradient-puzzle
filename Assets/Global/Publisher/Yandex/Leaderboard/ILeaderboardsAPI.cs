namespace Global.Publisher.Yandex
{
    public interface ILeaderboardsAPI
    {
        void SetLeaderboard_Internal(string target, int score);
        void GetLeaderboard_Internal(string target, int quantityTop, int quantityAround);
    }
}