namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDovePauseMusicCommand
    {
        internal static void Execute() => DDoveAudioKit.MusicPlayerOrNull?.Pause();
    }
}
