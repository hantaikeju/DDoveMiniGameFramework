using System;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePlayVoiceCommand
    {
        internal static void Execute(string voiceName, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null)
        {
            if (!DDoveAudioKit.TryGetRoot(out var root))
            {
                return;
            }

            DDoveAudioKit.EnsureAudioListener();
            DDoveAudioKit.CurrentVoiceName = voiceName;
            if (!DDoveAudioKit.Settings.IsVoiceOn)
            {
                return;
            }

            DDoveAudioKit.VoicePlayer
                .OnStart(onBeganCallback)
                .PrepareByNameAsyncAndPlay(root.gameObject, voiceName, loop)
                .OnFinish(onEndedCallback);
        }
    }
}
