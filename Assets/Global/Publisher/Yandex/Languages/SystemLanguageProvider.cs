namespace Global.Publisher
{
    public class SystemLanguageProvider : ISystemLanguageProvider
    {
        public SystemLanguageProvider(ILanguageAPI api)
        {
            _externAPI = api;
        }

        private readonly ILanguageAPI _externAPI;

        private bool _isLanguageReceived;
        private Language _selected;

        public Language GetLanguage()
        {
            if (_isLanguageReceived == true)
                return _selected;

            var raw = _externAPI.GetLanguage_Internal();
            _isLanguageReceived = true;

            return raw switch
            {
                "ru" => Language.Ru,
                "en" => Language.Eng,
                _ => Language.Ru
            };
        }
    }
}