using System;
using System.Collections.Generic;
using Global.Publisher;

namespace Global.Saves
{
    [Serializable]
    public class LevelsSave
    {
        public Dictionary<int, int> Passed { get; set; } = new();
    }
}