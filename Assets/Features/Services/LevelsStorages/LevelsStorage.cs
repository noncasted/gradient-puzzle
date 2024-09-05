using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.Publisher;
using Global.Saves;
using Internal;

namespace Features.Services
{
    public class LevelsStorage : ILevelsStorage, IScopeSetupAsync
    {
        public LevelsStorage(IDataStorage dataStorage, LevelsStorageOptions options)
        {
            _dataStorage = dataStorage;
            _configurations = options.Objects;
        }

        private readonly IDataStorage _dataStorage;
        private readonly IReadOnlyList<LevelConfiguration> _configurations;

        private LevelsSave _save;

        public IReadOnlyList<ILevelConfiguration> Configurations => _configurations;

        public async UniTask OnSetupAsync(IReadOnlyLifetime lifetime)
        {
            _save = await _dataStorage.GetEntry<LevelsSave>();

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

            if (_save.Unlocked >= index)
                return;

            _save.Unlocked = index;
            _dataStorage.Save(_save).Forget();
        }
    }
}