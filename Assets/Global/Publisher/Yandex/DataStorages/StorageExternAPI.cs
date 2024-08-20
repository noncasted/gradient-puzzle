using System.Runtime.InteropServices;

namespace Global.Publisher.Yandex
{
    public class StorageExternAPI : IStorageAPI
    {
        [DllImport("__Internal")]
        private static extern void GetUserData();

        [DllImport("__Internal")]
        private static extern void SaveUserData(string data);

        public void Get_Internal()
        {
            GetUserData();
        }

        public void Set_Internal(string data)
        {
            SaveUserData(data);
        }
    }
}