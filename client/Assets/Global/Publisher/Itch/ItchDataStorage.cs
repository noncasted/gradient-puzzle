using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Internal;
using Newtonsoft.Json;
using UnityEngine;

namespace Global.Publisher.Itch
{
    public class ItchDataStorage : IDataStorage, IScopeBaseSetup
    {
        public ItchDataStorage(DataStorageEventLoop eventLoop)
        {
            _eventLoop = eventLoop;
        }

        private const string Key = "save";
        
        private readonly Dictionary<string, string> _entries = new();
        private readonly DataStorageEventLoop _eventLoop;

        public void OnBaseSetup(IReadOnlyLifetime lifetime)
        {
            if (PlayerPrefs.HasKey(Key) == true)
            {
                var raw = PlayerPrefs.GetString(Key);
                var rawEntries = JsonConvert.DeserializeObject<Dictionary<string, string>>(raw);
                
                foreach (var (key, rawEntry) in rawEntries)
                    _entries.Add(key, rawEntry);
            }
            
            _eventLoop.OnDataStorageLoaded(lifetime, this).Forget();
        }

        public T Get<T>() where T : class, new()
        {
            var key = typeof(T).FullName!; 
            
            if (_entries.TryGetValue(key, out var rawEntry) == false)
                return new T();
            
            return JsonConvert.DeserializeObject<T>(rawEntry);
        }

        public UniTask Save<T>(T data)
        {
            var key = typeof(T).FullName!;
            var json = JsonConvert.SerializeObject(data);
            _entries[key] = json;
            PlayerPrefs.SetString(Key, JsonConvert.SerializeObject(_entries));
            return UniTask.CompletedTask;
        }
    }
}