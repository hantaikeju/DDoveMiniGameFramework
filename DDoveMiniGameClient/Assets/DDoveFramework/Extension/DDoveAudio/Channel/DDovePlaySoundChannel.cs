namespace DDoveFramework.Extension.DDoveAudio
{
    internal abstract class DDovePlaySoundChannel
    {
        internal abstract bool CanPlaySound(string soundName);
        internal abstract void SoundFinish(string soundName);
    }
}
