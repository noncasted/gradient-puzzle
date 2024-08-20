using System.Collections.Generic;
using Global.UI.Texts;

namespace Global.UI.Service
{
    public class LocalizationStorage : ILocalizationStorage
    {
        public LocalizationStorage(LanguageTextDataRegistry registry)
        {
            _registry = registry;
        }

        private readonly LanguageTextDataRegistry _registry;
        
        public IReadOnlyList<LanguageTextData> GetDatas()
        {
            return _registry.Objects;
        }
    }
}