using System;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePlayMusicWithClipCommand
    {
        internal static void Execute(AudioClip clip, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f)
        {
            if (clip == null || !DDoveAudioKit.TryGetRoot(out var root))
            {
                return;
            }

            DDoveAudioKit.EnsureAudioListener();
            DDoveAudioKit.CurrentMusicName = "music" + clip.GetHashCode();
            DDoveAudioKit.MusicPlayer
                .VolumeScale(volume)
                .OnStart(onBeganCallback)
                .PrepareByClipAndPlay(root.gameObject, clip, DDoveAudioKit.CurrentMusicName, loop)
                .OnFinish(onEndCallback);
        }
    }
}
