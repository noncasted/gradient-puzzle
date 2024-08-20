using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Internal;
using Newtonsoft.Json;

namespace Global.Publisher
{
    public class YandexDataStorage : IDataStorage, IScopeBaseSetupAsync
    {
        public YandexDataStorage(
            DataStorageEventLoop eventLoop,
            YandexCallbacks callbacks,
            IStorageAPI api,
            IReadOnlyList<IStorageEntrySerializer> entries)
        {
            _eventLoop = eventLoop;
            _callbacks = callbacks;
            _api = api;

            foreach (var entry in entries)
            {
                _typeToSerializer.Add(entry.ValueType, entry);
                _keyToSerializer.Add(entry.SaveKey, entry);
            }
        }

        private readonly Dictionary<Type, IStorageEntrySerializer> _typeToSerializer = new();
        private readonly Dictionary<string, IStorageEntrySerializer> _keyToSerializer = new();

        private readonly DataStorageEventLoop _eventLoop;
        private readonly YandexCallbacks _callbacks;
        private readonly IStorageAPI _api;

        public async UniTask OnBaseSetupAsync(IReadOnlyLifetime lifetime)
        {
            var completion = new UniTaskCompletionSource<string>();

            _callbacks.UserDataReceived += OnDataReceived;

            await UniTask.WaitUntil(() => _callbacks.IsInitialized == true);

            _api.Get_Internal();
            var data = await completion.Task;

            _callbacks.UserDataReceived -= OnDataReceived;

            var rawEntries = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            foreach (var (key, rawData) in rawEntries)
                _keyToSerializer[key].Deserialize(rawData);

            await _eventLoop.OnDataStorageLoaded(lifetime, this);

            return;

            void OnDataReceived(string raw)
            {
                completion.TrySetResult(raw);
            }
        }

        public UniTask<T> GetEntry<T>() where T : class
        {
            var type = typeof(T);
            var entry = _typeToSerializer[type].Get<T>();

            return UniTask.FromResult(entry);
        }

        public UniTask Save<T>(T data)
        {
            var type = typeof(T);
            _typeToSerializer[type].Set(data);

            var save = new Dictionary<string, string>();

            foreach (var (key, entry) in _keyToSerializer)
            {
                var rawEntry = entry.Serialize();
                save[key] = rawEntry;
            }

            var json = JsonConvert.SerializeObject(save);
            _api.Set_Internal(json);
            return UniTask.CompletedTask;
        }
    }
}