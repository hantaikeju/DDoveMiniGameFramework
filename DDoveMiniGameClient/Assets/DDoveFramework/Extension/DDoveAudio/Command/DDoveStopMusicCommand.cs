namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDoveStopMusicCommand
    {
        internal static void Execute() => DDoveAudioKit.MusicPlayerOrNull?.Stop();
    }
}
