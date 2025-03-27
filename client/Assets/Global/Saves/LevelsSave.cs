using System;
using System.Collections.Generic;
using Global.Publisher;

namespace Global.Saves
{
    [Serializable]
    public class LevelsSave
    {
        public HashSet<string> Passed { get; set; } = new();
    }

    public class LevelsSaveSerializer : StorageEntrySerializer<LevelsSave>
    {
        public LevelsSaveSerializer() : base("levels", new LevelsSave())
        {
        }
    }
}