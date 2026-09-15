namespace DDoveFramework.Extension.DDoveAudio
{
    public struct AudioSourceProxySourceVolumeChangeEvent
    {
        public float Volume;
        public int SourceHashCode;
    }

    public struct MusicOnOffChangeEvent
    {
        public bool IsOn;
    }

    public struct VoiceOnOffChangeEvent
    {
        public bool IsOn;
    }

    public struct SoundOnOffChangeEvent
    {
        public bool IsOn;
    }

    public struct MusicVolumeChangeEvent
    {
        public float VolumeValue;
    }

    public struct VoiceVolumeChangeEvent
    {
        public float VolumeValue;
    }

    public struct SoundVolumeChangeEvent
    {
        public float VolumeValue;
    }
}
