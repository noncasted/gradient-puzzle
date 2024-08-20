using System.Collections.Generic;
using Global.UI.Texts;

namespace Global.UI.Service
{
    public interface ILocalizationStorage
    {
        IReadOnlyList<LanguageTextData> GetDatas();
    }
}