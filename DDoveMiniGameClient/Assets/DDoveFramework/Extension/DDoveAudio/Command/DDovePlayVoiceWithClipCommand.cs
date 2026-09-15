using System;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePlayVoiceWithClipCommand
    {
        internal static void Execute(AudioClip clip, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null, float volumeScale = 1.0f)
        {
            if (clip == null || !DDoveAudioKit.TryGetRoot(out var root))
            {
                return;
            }

            DDoveAudioKit.EnsureAudioListener();
            DDoveAudioKit.CurrentVoiceName = "voice" + clip.GetHashCode();
            if (!DDoveAudioKit.Settings.IsVoiceOn)
            {
                return;
            }

            DDoveAudioKit.VoicePlayer
                .VolumeScale(volumeScale)
                .OnStart(onBeganCallback)
                .PrepareByClipAndPlay(root.gameObject, clip, DDoveAudioKit.CurrentVoiceName, loop)
                .OnFinish(onEndedCallback);
        }
    }
}
