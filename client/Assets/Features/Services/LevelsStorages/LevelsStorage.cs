using System;
using System.Collections.Generic;
using System.Linq;
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
            LevelOptionsRegistry optionsRegistry)
        {
            _optionsRegistry = optionsRegistry;
            _dataStorage = dataStorage;
            _userBackend = userBackend;
        }

        private readonly IDataStorage _dataStorage;
        private readonly IUserBackend _userBackend;

        private LevelsSave _save;
        private IReadOnlyLifetime _lifetime;
        private readonly Dictionary<LevelSectionType, IReadOnlyList<ILevelData>> _sections = new();
        private LevelOptionsRegistry _optionsRegistry;

        public IReadOnlyDictionary<LevelSectionType, IReadOnlyList<ILevelData>> Sections => _sections;

        public async UniTask OnSetupAsync(IReadOnlyLifetime lifetime)
        {
            _lifetime = lifetime;
            _save = _dataStorage.Get<LevelsSave>();

            var sections = new Dictionary<LevelSectionType, List<ILevelData>>();

            var sectionTypes = Enum.GetValues(typeof(LevelSectionType)).Cast<LevelSectionType>();

            foreach (var sectionType in sectionTypes)
                sections.Add(sectionType, new List<ILevelData>());

            foreach (var level in _optionsRegistry.Objects)
            {
                var list = sections[level.SectionType];
                var data = new LevelData(level, list.Count);
                list.Add(data);
            }

            foreach (var (type, list) in sections)
                _sections.Add(type, list);

            foreach (var (type, list) in sections)
            {
                var sectionKey = (int)type;

                if (_save.Passed.TryGetValue(sectionKey, out var index) == false)
                    continue;

                foreach (var data in list)
                {
                    if (data.Index > index)
                        continue;

                    data.Unlock();
                }
            }
        }

        public void OnLevelPassed(ILevelData data)
        {
            if (data.IsUnlocked.Value == true)
                return;

            _sections[data.SectionType][data.Index].OnPassed();
            _save.Passed[(int)data.SectionType] = data.Index;
            _dataStorage.Save(_save).Forget();
            _userBackend.SaveProgress(_lifetime, (int)data.SectionType, data.Index).Forget();

            RecalculateUnlocks().Forget();
        }

        public async UniTask RecalculateUnlocks()
        {
            _save = _dataStorage.Get<LevelsSave>();

            foreach (var (type, list) in _sections)
            {
                var sectionKey = (int)type;

                if (_save.Passed.TryGetValue(sectionKey, out var index) == false)
                    continue;

                foreach (var data in list)
                {
                    if (data.Index > index)
                        continue;

                    data.OnPassed();
                }
            }

            foreach (var (type, list) in _sections)
            {
                var sectionKey = (int)type;

                if (_save.Passed.TryGetValue(sectionKey, out var index) == false)
                    continue;

                if (index < list.Count - 1)
                {
                    var next = list[index + 1];
                    next.Unlock();
                }
            }
        }
    }
}