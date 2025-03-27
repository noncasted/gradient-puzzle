using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.GameServices;
using Internal;
using Newtonsoft.Json;
using Shared;
using UnityEngine;

namespace Global.Backend
{
    public class UserBackend : IUserBackend
    {
        public UserBackend(
            IBackendClient client,
            IGlobalContext globalContext,
            BackendOptions backendOptions)
        {
            _client = client;
            _globalContext = globalContext;
            _backendOptions = backendOptions;
        }

        private readonly IBackendClient _client;
        private readonly IGlobalContext _globalContext;
        private readonly BackendOptions _backendOptions;

        public async UniTask<IReadOnlyList<string>> GetProgress(IReadOnlyLifetime lifetime)
        {
            var request = new GetUserProgress.Request()
            {
                UserId = _globalContext.UserId
            };

            var url = _backendOptions.Url + "/getProgress";

            var test = new ASD()
            {
                BB = 912
            };
            
            var testJson = JsonConvert.SerializeObject(test);
            var testResult = JsonConvert.DeserializeObject<ASD>(testJson);
            
            Debug.Log($"[Backend] test: {testResult.BB}");
            
            var bodyJson = JsonConvert.SerializeObject(request);
            Debug.Log($"[Backend] Get player progress: {bodyJson}");

            var response = await _client.PostJson<GetUserProgress.Response>(lifetime, url, request);
            
            return response.PassedLevels;
        }

        public UniTask SaveProgress(IReadOnlyLifetime lifetime, string passedLevelId)
        {
            var request = new SetUserProgress.Request()
            {
                UserId = _globalContext.UserId,
                LevelId = passedLevelId
            };

            var url = _backendOptions.Url + "/setLevelPassed";

            return _client.PostJson(lifetime, url, request);
        }

        public class ASD
        {
            public int BB;
        }
    }
}