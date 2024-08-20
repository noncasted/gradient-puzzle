namespace Global.Publisher.Yandex
{
    public class LeaderboardResponse
    {
        public Leaderboard Leaderboard;
        public LeaderboardEntry[] Entries;
    }

    public class Leaderboard
    {
        public string LeaderboardName;
    }

    public class LeaderboardEntry
    {
        public int Score;
        public int Rank;

        public LeaderboardPlayer Player;
    }

    public class LeaderboardPlayer
    {
        public string PublicName;
    }
}