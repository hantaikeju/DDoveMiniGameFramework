namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDoveResumeMusicCommand
    {
        internal static void Execute() => DDoveAudioKit.MusicPlayerOrNull?.Resume();
    }
}
