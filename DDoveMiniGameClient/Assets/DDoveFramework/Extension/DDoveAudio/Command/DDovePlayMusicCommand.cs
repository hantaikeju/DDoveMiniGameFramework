using System;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePlayMusicCommand
    {
        internal static void Execute(string musicName, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f)
        {
            if (!DDoveAudioKit.TryGetRoot(out var root))
            {
                return;
            }

            DDoveAudioKit.EnsureAudioListener();
            DDoveAudioKit.CurrentMusicName = musicName;
            DDoveAudioKit.MusicPlayer
                .VolumeScale(volume)
                .OnStart(onBeganCallback)
                .PrepareByNameAsyncAndPlay(root.gameObject, musicName, loop)
                .OnFinish(onEndCallback);
        }
    }
}
