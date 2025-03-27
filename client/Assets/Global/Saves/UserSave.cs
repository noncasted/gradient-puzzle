using System;
using Global.Publisher;

namespace Global.Saves
{
    [Serializable]
    public class UserSave
    {
        public Guid UserId { get; set; }
    }
    
    public class UserSaveSerializer : StorageEntrySerializer<UserSave>
    {
        public UserSaveSerializer() : base("user", new UserSave())
        {
        }
    }
}