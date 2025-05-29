using System;
using System.Collections.Generic;
using Global.Audio;

namespace Global.Saves
{
    [Serializable]
    public class VolumeSave
    {
        public readonly Dictionary<AudioLine, float> Values = new()
        {
            { AudioLine.Music, 0.5f },
            { AudioLine.SFX, 0.5f }
        };
    }
}