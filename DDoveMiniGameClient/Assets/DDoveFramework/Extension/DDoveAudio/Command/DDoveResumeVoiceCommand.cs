namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDoveResumeVoiceCommand
    {
        internal static void Execute() => DDoveAudioKit.VoicePlayerOrNull?.Resume();
    }
}
