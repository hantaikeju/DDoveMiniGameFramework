namespace DDoveFramework.Extension.DDoveAudio
{
    internal static class DDoveStopAllSoundCommand
    {
        internal static void Execute()
        {
            DDoveAudioKit.PlayingSoundPool.ForEachAllSound(player => player.Stop());
            DDoveAudioKit.PlayingSoundPool.ClearAllPlayingSound();
        }
    }
}
