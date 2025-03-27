using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.Backend;
using Global.Publisher;
using Global.Saves;
using Internal;

namespace Services
{
    public class LevelsStorage : ILevelsStorage, IScopeSetupAsync
    {
        public LevelsStorage(
            IDataStorage dataStorage,
            IUserBackend userBackend,
            LevelsStorageOptions options)
        {
            _dataStorage = dataStorage;
            _userBackend = userBackend;
            _configurations = options.Objects;
        }

        private readonly IDataStorage _dataStorage;
        private readonly IUserBackend _userBackend;
        private readonly IReadOnlyList<LevelConfiguration> _configurations;

        private LevelsSave _save;
        private IReadOnlyLifetime _lifetime;

        public IReadOnlyList<ILevelConfiguration> Configurations => _configurations;

        public async UniTask OnSetupAsync(IReadOnlyLifetime lifetime)
        {
            _lifetime = lifetime;
            _save = await _dataStorage.GetEntry<LevelsSave>();
            _save.Passed = new HashSet<string>();

            for (var i = 0; i < _configurations.Count; i++)
                _configurations[i].Setup(i + 1);

            for (var i = 0; i < _configurations.Count; i++)
            {
                // if (i >= _save.Unlocked)
                //     break;

                var configuration = _configurations[i];
                configuration.OnUnlocked();
            }
        }

        public void OnLevelPassed(ILevelConfiguration configuration)
        {
            var index = configuration.Index;
            _configurations[index].OnUnlocked();

            _save.Passed.Add(configuration.Id);
            _dataStorage.Save(_save).Forget();
            _userBackend.SaveProgress(_lifetime, configuration.Id).Forget();
        }
    }
}