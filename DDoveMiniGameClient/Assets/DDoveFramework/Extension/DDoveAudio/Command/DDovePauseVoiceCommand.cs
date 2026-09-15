namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePauseVoiceCommand
    {
        internal static void Execute() => DDoveAudioKit.VoicePlayerOrNull?.Pause();
    }
}
