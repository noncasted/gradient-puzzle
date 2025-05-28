using System;
using System.Collections.Generic;
using Global.Publisher;
using Shared;

namespace Global.Saves
{
    [Serializable]
    public class LevelsSave
    {
        public Dictionary<LevelSectionType, int> Passed { get; set; } = new();
    }
}