using System;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePlayVoiceOnceCommand
    {
        internal static DDoveAudioPlayer Execute(string voiceName, Action<DDoveAudioPlayer> callBack = null,
            float volumeScale = 1.0f)
        {
            if (string.IsNullOrEmpty(voiceName) || !DDoveAudioKit.TryGetRoot(out var root))
            {
                return null;
            }

            DDoveAudioKit.EnsureAudioListener();
            var player = DDoveAudioPlayer.Allocate(DDoveAudioKit.Settings.VoiceVolume);
            player
                .VolumeScale(volumeScale)
                .PrepareByNameAsyncAndPlay(root.gameObject, voiceName, false);
            player.OnFinish(() => callBack?.Invoke(player));
            return player;
        }
    }
}
