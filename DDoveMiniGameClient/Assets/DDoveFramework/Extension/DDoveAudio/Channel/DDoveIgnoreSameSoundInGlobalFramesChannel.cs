using System.Collections.Generic;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDoveIgnoreSameSoundInGlobalFramesChannel : DDovePlaySoundChannel
    {
        private readonly Dictionary<string, int> _soundFrameCountForName = new Dictionary<string, int>();
        private int _globalFrameCount;

        internal int GlobalFrameCountForIgnoreSameSound = 10;

        internal override bool CanPlaySound(string soundName)
        {
            if (Time.frameCount - _globalFrameCount <= GlobalFrameCountForIgnoreSameSound)
            {
                if (_soundFrameCountForName.ContainsKey(soundName))
                {
                    return false;
                }

                _soundFrameCountForName.Add(soundName, 0);
            }
            else
            {
                _globalFrameCount = Time.frameCount;
                _soundFrameCountForName.Clear();
                _soundFrameCountForName.Add(soundName, 0);
            }

            return true;
        }

        internal override void SoundFinish(string soundName)
        {
        }
    }
}
