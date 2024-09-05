using System;
using Global.Publisher;

namespace Global.Saves
{
    [Serializable]
    public class LevelsSave
    {
        public int Unlocked { get; set; } = 1;
    }

    public class LevelsSaveSerializer : StorageEntrySerializer<LevelsSave>
    {
        public LevelsSaveSerializer() : base("levels", new LevelsSave())
        {
        }
    }
}