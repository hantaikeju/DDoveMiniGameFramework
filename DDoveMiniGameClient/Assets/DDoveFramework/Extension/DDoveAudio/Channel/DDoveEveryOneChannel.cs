namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDoveEveryOneChannel : DDovePlaySoundChannel
    {
        internal override bool CanPlaySound(string soundName) => true;

        internal override void SoundFinish(string soundName)
        {
        }
    }
}
