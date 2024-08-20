using UnityEngine;

namespace Global.Audio.Abstract
{
    public interface IGlobalAudioPlayer
    {
        void PlaySound(AudioClip clip);
        void PlayLoopMusic(AudioClip clip);
    }
}