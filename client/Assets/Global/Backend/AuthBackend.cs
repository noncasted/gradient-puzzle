using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Global.GameServices;
using Global.Publisher;
using Global.Saves;
using Internal;
using UnityEngine;

namespace Global.Backend
{
    public class AuthBackend : IAuthBackend
    {
        public AuthBackend(
            IBackendClient client,
            IDataStorage dataStorage,
            PlatformOptions platformOptions,
            BackendOptions options)
        {
            _client = client;
            _dataStorage = dataStorage;
            _platformOptions = platformOptions;
            _options = options;
        }

        private readonly IBackendClient _client;
        private readonly IDataStorage _dataStorage;
        private readonly PlatformOptions _platformOptions;
        private readonly BackendOptions _options;

        [DllImport("__Internal")]
        private static extern string GetUserIdFromUrl();

        public async UniTask<Guid> Auth(IReadOnlyLifetime lifetime)
        {
            var save = _dataStorage.Get<UserSave>();

            Debug.Log($"Current platform: {Application.platform}");

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                var rawUserId = GetUserIdFromUrl();

                if (string.IsNullOrEmpty(rawUserId) == true)
                    save.UserId = Guid.NewGuid();
                else
                    save.UserId = Guid.Parse(rawUserId);
            }

            if (save.UserId == Guid.Empty)
                save.UserId = Guid.NewGuid();

            await _dataStorage.Save(save);
            return save.UserId;
        }
    }
}