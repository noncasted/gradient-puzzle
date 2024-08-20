using System.Runtime.InteropServices;

namespace Global.Publisher
{
    public class ItchLanguageExternAPI : IItchLanguageAPI
    {
        [DllImport("__Internal")]
        private static extern string GetLanguageItch();

        public string GetLanguage_Internal()
        {
            return GetLanguageItch();
        }
    }
}