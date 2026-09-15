namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDoveStopVoiceCommand
    {
        internal static void Execute() => DDoveAudioKit.VoicePlayerOrNull?.Stop();
    }
}
