using Newtonsoft.Json;

namespace Global.Publisher
{
    public class LeaderboardsDebugAPI : ILeaderboardsAPI
    {
        public LeaderboardsDebugAPI(YandexCallbacks callbacks)
        {
            _callbacks = callbacks;
        }
        
        private readonly YandexCallbacks _callbacks;

        public void SetLeaderboard_Internal(string target, int score)
        {
        }

        public void GetLeaderboard_Internal(string target, int quantityTop, int quantityAround)
        {
            var leaderboard = new Leaderboard()
            {
                LeaderboardName = target
            };
            
            var entries = new LeaderboardEntry[]
            {
                new()
                {
                    Player = new LeaderboardPlayer()
                    {
                        PublicName = "Aboba_1"
                    },
                    Rank = 1,
                    Score = 9999999
                },
                new()
                {
                    Player = new LeaderboardPlayer()
                    {
                        PublicName = "Aboba_2"
                    },
                    Rank = 2,
                    Score = 9999998
                },
                new()
                {
                    Player = new LeaderboardPlayer()
                    {
                        PublicName = "Aboba_3"
                    },
                    Rank = 3,
                    Score = 9999997
                },
                new()
                {
                    Player = new LeaderboardPlayer()
                    {
                        PublicName = "Aboba_66"
                    },
                    Rank = 66,
                    Score = 123123
                },
                new()
                {
                    Player = new LeaderboardPlayer()
                    {
                        PublicName = "Aboba_65"
                    },
                    Rank = 65,
                    Score = 123122
                },
                new()
                {
                    Player = new LeaderboardPlayer()
                    {
                        PublicName = "Aboba_64"
                    },
                    Rank = 64,
                    Score = 123121
                }
            };

            var response = new LeaderboardResponse()
            {
                Leaderboard = leaderboard,
                Entries = entries
            };

            var raw = JsonConvert.SerializeObject(response);
            
            _callbacks.OnLeaderboardsReceived(raw);
        }
    }
}