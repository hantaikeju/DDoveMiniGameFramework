using System.Collections.Generic;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDoveIgnoreSameSoundInSoundFramesChannel : DDovePlaySoundChannel
    {
        private readonly Dictionary<string, int> _soundFrameCountForName = new Dictionary<string, int>();

        internal int SoundFrameCountForIgnoreSameSound = 10;

        internal override bool CanPlaySound(string soundName)
        {
            if (_soundFrameCountForName.TryGetValue(soundName, out var frames))
            {
                if (Time.frameCount - frames <= SoundFrameCountForIgnoreSameSound)
                {
                    return false;
                }

                _soundFrameCountForName[soundName] = Time.frameCount;
            }
            else
            {
                _soundFrameCountForName.Add(soundName, Time.frameCount);
            }

            return true;
        }

        internal override void SoundFinish(string soundName)
        {
            _soundFrameCountForName.Remove(soundName);
        }
    }
}
