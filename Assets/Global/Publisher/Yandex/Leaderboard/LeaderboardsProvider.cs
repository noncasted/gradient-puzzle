using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Global.Publisher.Yandex
{
    public class LeaderboardsProvider : ILeaderboardsProvider
    {
        public LeaderboardsProvider(ILeaderboardsAPI api, YandexCallbacks callbacks)
        {
            _api = api;
            _callbacks = callbacks;
        }

        private readonly ILeaderboardsAPI _api;
        private readonly YandexCallbacks _callbacks;

        public void SetScore(ILeaderboardLink link, int score)
        {
            var target = link.GetLeaderboardName();

            _api.SetLeaderboard_Internal(target, score);
        }

        public async UniTask<IReadOnlyList<LeaderboardUser>> GetLeaderBoard(
            ILeaderboardLink link,
            int quantityTop,
            int quantityAround,
            CancellationToken cancellation)
        {
            var completion = new UniTaskCompletionSource<LeaderboardResponse>();
            cancellation.Register(() => completion.TrySetException(new OperationCanceledException()));
            _callbacks.LeaderboardsReceived += OnReceived;

            _api.GetLeaderboard_Internal(link.GetLeaderboardName(), quantityTop, quantityAround);

            void OnReceived(string raw)
            {
                var response = JsonConvert.DeserializeObject<LeaderboardResponse>(raw);

                if (response == null)
                {
                    Debug.LogError($"Could not deserialize LeaderboardResponse from: {raw}");
                    completion.TrySetException(new OperationCanceledException());
                    return;
                }
                
                completion.TrySetResult(response);
            }

            var response = await completion.Task;
            
            _callbacks.LeaderboardsReceived -= OnReceived;

            return ParseResponse(response);
        }

        private IReadOnlyList<LeaderboardUser> ParseResponse(LeaderboardResponse response)
        {
            var list = new List<LeaderboardUser>();

            foreach (var entry in response.Entries)
            {
                var user = new LeaderboardUser()
                {
                    PlayerName = entry.Player.PublicName,
                    Rank = entry.Rank,
                    Score = entry.Score
                };

                list.Add(user);
            }

            return list;
        }
    }
}