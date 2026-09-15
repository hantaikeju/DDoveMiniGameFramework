using System;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePlaySoundWithClipCommand
    {
        internal static DDoveAudioPlayer Execute(AudioClip clip, bool loop = false,
            Action<DDoveAudioPlayer> callBack = null, float volume = 1.0f, float pitch = 1,
            DDovePlaySoundModes? playSoundMode = null)
        {
            if (clip == null || !DDoveAudioKit.TryGetRoot(out var root))
            {
                return null;
            }

            DDoveAudioKit.EnsureAudioListener();
            var soundName = string.IsNullOrWhiteSpace(clip.name)
                ? "AudioClip:" + clip.GetHashCode()
                : clip.name;
            var soundPlayer = DDoveAudioPlayer.Allocate(DDoveAudioKit.Settings.SoundVolume, playSoundMode);
            soundPlayer.OnFinish(() => callBack?.Invoke(soundPlayer));
            soundPlayer
                .VolumeScale(volume)
                .PrepareByClipAndPlay(root.gameObject, clip, soundName, loop)
                .Pitch(pitch);
            return soundPlayer;
        }
    }
}
