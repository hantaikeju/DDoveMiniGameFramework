using System;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePlaySoundCommand
    {
        internal static DDoveAudioPlayer Execute(string soundName, bool loop = false,
            Action<DDoveAudioPlayer> callBack = null, float volume = 1.0f, float pitch = 1,
            DDovePlaySoundModes? playSoundMode = null)
        {
            if (!DDoveAudioKit.TryGetRoot(out var root))
            {
                return null;
            }

            DDoveAudioKit.EnsureAudioListener();
            var soundPlayer = DDoveAudioPlayer.Allocate(DDoveAudioKit.Settings.SoundVolume, playSoundMode);
            soundPlayer
                .VolumeScale(volume)
                .PrepareByNameAsyncAndPlay(root.gameObject, soundName, loop)
                .Pitch(pitch);
            soundPlayer.OnFinish(() => callBack?.Invoke(soundPlayer));
            return soundPlayer;
        }
    }
}
