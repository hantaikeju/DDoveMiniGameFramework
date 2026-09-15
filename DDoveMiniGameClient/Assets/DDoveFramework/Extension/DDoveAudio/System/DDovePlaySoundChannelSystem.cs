using DDoveFramework.Core;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDovePlaySoundChannelSystem : AbstractSystem
    {
        internal readonly DDoveEveryOneChannel EveryOneChannel = new DDoveEveryOneChannel();

        internal readonly DDoveIgnoreSameSoundInSoundFramesChannel IgnoreInSoundFramesChannel =
            new DDoveIgnoreSameSoundInSoundFramesChannel();

        internal readonly DDoveIgnoreSameSoundInGlobalFramesChannel IgnoreInGlobalFramesChannel =
            new DDoveIgnoreSameSoundInGlobalFramesChannel();

        internal bool CanPlaySound(DDoveAudioPlayer player)
        {
            switch (player.PlaySoundMode)
            {
                case DDovePlaySoundModes.EveryOne:
                    return EveryOneChannel.CanPlaySound(player.AudioName);
                case DDovePlaySoundModes.IgnoreSameSoundInSoundFrames:
                    return IgnoreInSoundFramesChannel.CanPlaySound(player.AudioName);
                case DDovePlaySoundModes.IgnoreSameSoundInGlobalFrames:
                    return IgnoreInGlobalFramesChannel.CanPlaySound(player.AudioName);
                default:
                    return true;
            }
        }

        internal void SoundFinish(DDoveAudioPlayer player)
        {
            switch (player.PlaySoundMode)
            {
                case DDovePlaySoundModes.EveryOne:
                    EveryOneChannel.SoundFinish(player.AudioName);
                    break;
                case DDovePlaySoundModes.IgnoreSameSoundInSoundFrames:
                    IgnoreInSoundFramesChannel.SoundFinish(player.AudioName);
                    break;
                case DDovePlaySoundModes.IgnoreSameSoundInGlobalFrames:
                    IgnoreInGlobalFramesChannel.SoundFinish(player.AudioName);
                    break;
            }
        }

        protected override void OnInit()
        {
        }
    }
}
