using DDoveFramework.Core;

namespace DDoveFramework.Extension.DDoveAudio
{
    public sealed class DDoveAudioArchitecture : Architecture<DDoveAudioArchitecture>
    {
        protected override void Init()
        {
            RegisterSystem(new DDovePlaySoundChannelSystem());
            RegisterModel(new DDoveAudioKitSettingsModel());
            RegisterModel(new DDovePlayingSoundPoolModel());
        }
    }
}
